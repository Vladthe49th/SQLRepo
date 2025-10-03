#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <ctime>
#include <cstdlib>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

// --- Глобальні змінні ---
std::map<std::string, std::string> users;       // username -> password
std::map<SOCKET, std::string> loggedIn;         // socket -> username
std::vector<SOCKET> clients;

std::map<std::string, std::vector<SOCKET>> rooms; // roomname -> список сокетів
std::map<SOCKET, std::string> userRoom;          // клієнт -> кімната

std::map<std::string, bool> bannedUsers;        // username -> banned
std::map<std::string, bool> admins;             // username -> admin

// --- Цензура ---
std::vector<std::string> badWords = { "badword", "curse" };
std::map<char, std::string> morse = {
    {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."},
    {'F', "..-."}, {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"},
    {'K', "-.-"}, {'L', ".-.."}, {'M', "--"}, {'N', "-."}, {'O', "---"},
    {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."}, {'S', "..."}, {'T', "-"},
    {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"}, {'Y', "-.--"},
    {'Z', "--.."}, {'0',"-----"},{'1',".----"},{'2',"..---"},{'3',"...--"},
    {'4',"....-"},{'5',"....."},{'6',"-...."},{'7',"--..."},{'8',"---.."},
    {'9',"----."}
};

const std::string USERS_FILE = "users.txt";
const std::string BANNED_FILE = "banned.txt";

// --- Завантаження користувачів ---
void loadUsers() {
    std::ifstream fin(USERS_FILE);
    if (!fin.is_open()) return;
    std::string user, pass;
    while (fin >> user >> pass) users[user] = pass;
    fin.close();
    std::cout << "Loaded " << users.size() << " users.\n";
}

// --- Збереження нового користувача ---
void saveUser(const std::string& user, const std::string& pass) {
    std::ofstream fout(USERS_FILE, std::ios::app);
    fout << user << " " << pass << "\n";
    fout.close();
}

// --- Завантаження заблокованих користувачів ---
void loadBanned() {
    std::ifstream fin(BANNED_FILE);
    if (!fin.is_open()) return;
    std::string user;
    while (fin >> user) bannedUsers[user] = true;
    fin.close();
}

// --- Збереження бану ---
void saveBanned() {
    std::ofstream fout(BANNED_FILE);
    for (auto it = bannedUsers.begin(); it != bannedUsers.end(); ++it) {
        fout << it->first << "\n";
    }
    fout.close();
}

// --- Морзе ---
std::string toMorse(const std::string& word) {
    std::string result;
    for (char c : word) {
        char up = toupper(c);
        if (morse.count(up)) result += morse[up] + " ";
        else result += c;
    }
    return result;
}

// --- Цензура ---
std::string censorMessage(const std::string& msg) {
    std::string censored = msg;
    for (size_t i = 0; i < badWords.size(); ++i) {
        const std::string& word = badWords[i];
        size_t pos = 0;
        while ((pos = censored.find(word, pos)) != std::string::npos) {
            std::string replacement = toMorse(word);
            censored.replace(pos, word.length(), replacement);
            pos += replacement.length();
        }
    }
    return censored;
}

// --- Розсилка повідомлень ---
void broadcastRoom(const std::string& message, const std::string& roomName, SOCKET sender) {
    for (size_t i = 0; i < rooms[roomName].size(); ++i) {
        SOCKET sock = rooms[roomName][i];
        if (sock != sender) send(sock, message.c_str(), (int)message.size(), 0);
    }
}

int main() {
    srand((unsigned int)time(0));

    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(5555);

    bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    listen(serverSocket, 5);
    std::cout << "Server started...\n";

    // --- Завантаження даних ---
    loadUsers();
    loadBanned();

    // --- Адміни ---
    admins["admin"] = true;

    fd_set readfds;

    while (true) {
        FD_ZERO(&readfds);
        FD_SET(serverSocket, &readfds);
        for (size_t i = 0; i < clients.size(); ++i) FD_SET(clients[i], &readfds);

        int activity = select(0, &readfds, NULL, NULL, NULL);
        if (activity == SOCKET_ERROR) break;

        if (FD_ISSET(serverSocket, &readfds)) {
            SOCKET newClient = accept(serverSocket, NULL, NULL);
            clients.push_back(newClient);
            send(newClient, "Welcome! REGISTER or LOGIN\n", 27, 0);
        }

        for (size_t i = 0; i < clients.size(); ++i) {
            SOCKET client = clients[i];
            if (!FD_ISSET(client, &readfds)) continue;

            char buffer[1024] = { 0 };
            int bytesRead = recv(client, buffer, sizeof(buffer), 0);
            if (bytesRead <= 0) {
                closesocket(client);
                clients.erase(clients.begin() + i);
                loggedIn.erase(client);
                if (userRoom.count(client)) {
                    std::string roomName = userRoom[client];
                    auto& vec = rooms[roomName];
                    vec.erase(std::remove(vec.begin(), vec.end(), client), vec.end());
                    userRoom.erase(client);
                }
                i--;
                continue;
            }

            std::string msg(buffer, bytesRead);
            msg = censorMessage(msg);

            // --- REGISTER ---
            if (msg.rfind("REGISTER", 0) == 0) {
                char u[256], p[256];
                if (sscanf(msg.c_str(), "REGISTER %s %s", u, p) == 2) {
                    std::string user = u, pass = p;
                    if (users.count(user)) send(client, "User exists\n", 12, 0);
                    else { users[user] = pass; saveUser(user, pass); send(client, "Registered\n", 11, 0); }
                }
            }
            // --- LOGIN ---
            else if (msg.rfind("LOGIN", 0) == 0) {
                char u[256], p[256];
                if (sscanf(msg.c_str(), "LOGIN %s %s", u, p) == 2) {
                    std::string user = u, pass = p;
                    if (bannedUsers[user]) send(client, "You are banned\n", 15, 0);
                    else if (users.count(user) && users[user] == pass) { loggedIn[client] = user; send(client, "Login successful\n", 17, 0); }
                    else send(client, "Invalid login\n", 14, 0);
                }
            }
            // --- ADMIN: /ban ---
            else if (msg.rfind("/ban", 0) == 0) {
                if (!loggedIn.count(client)) { send(client, "Login first\n", 12, 0); continue; }
                std::string adminUser = loggedIn[client];
                if (!admins[adminUser]) { send(client, "Not admin\n", 10, 0); continue; }

                std::istringstream iss(msg); std::string cmd, targetUser; iss >> cmd >> targetUser;
                if (targetUser.empty()) { send(client, "Usage: /ban <user>\n", 20, 0); continue; }

                bannedUsers[targetUser] = true; saveBanned();

                for (size_t j = 0; j < clients.size(); ++j) {
                    SOCKET sock = clients[j];
                    if (loggedIn.count(sock) && loggedIn[sock] == targetUser) {
                        send(sock, "You are banned by admin\n", 25, 0);
                        closesocket(sock); clients.erase(clients.begin() + j); loggedIn.erase(sock);
                        if (userRoom.count(sock)) {
                            std::string roomName = userRoom[sock];
                            auto& vec = rooms[roomName];
                            vec.erase(std::remove(vec.begin(), vec.end(), sock), vec.end());
                            userRoom.erase(sock);
                        }
                        break;
                    }
                }
                send(client, ("User " + targetUser + " banned\n").c_str(), 18 + targetUser.size(), 0);
            }
            // --- JOIN ROOM ---
            else if (msg.rfind("/join", 0) == 0) {
                if (!loggedIn.count(client)) { send(client, "Login first\n", 12, 0); continue; }
                std::string cmd, roomName; std::istringstream iss(msg); iss >> cmd >> roomName;
                if (roomName.empty()) { send(client, "Usage: /join <room>\n", 22, 0); continue; }

                if (userRoom.count(client)) {
                    auto& vec = rooms[userRoom[client]];
                    vec.erase(std::remove(vec.begin(), vec.end(), client), vec.end());
                }
                rooms[roomName].push_back(client); userRoom[client] = roomName;
                send(client, ("Joined room: " + roomName + "\n").c_str(), 14 + roomName.size(), 0);
            }
            // --- LEAVE ROOM ---
            else if (msg.rfind("/leave", 0) == 0) {
                if (userRoom.count(client)) {
                    std::string roomName = userRoom[client];
                    auto& vec = rooms[roomName];
                    vec.erase(std::remove(vec.begin(), vec.end(), client), vec.end());
                    userRoom.erase(client);
                    send(client, "Left room\n", 10, 0);
                }
                else send(client, "Not in a room\n", 14, 0);
            }
            // --- LIST ROOMS ---
            else if (msg.rfind("/rooms", 0) == 0) {
                std::string list = "Rooms:\n";
                for (auto it = rooms.begin(); it != rooms.end(); ++it)
                    list += "- " + it->first + " (" + std::to_string(it->second.size()) + ")\n";
                send(client, list.c_str(), (int)list.size(), 0);
            }
            // --- PRIVATE MESSAGE ---
            else if (msg.rfind("/w", 0) == 0) {
                if (!loggedIn.count(client)) { send(client, "Login first\n", 12, 0); continue; }
                std::istringstream iss(msg); std::string cmd, targetUser; iss >> cmd >> targetUser;
                std::string message; std::getline(iss, message);
                if (!message.empty() && message.front() == ' ') message.erase(0, 1);

                SOCKET targetSock = INVALID_SOCKET;
                for (auto it = loggedIn.begin(); it != loggedIn.end(); ++it) {
                    if (it->second == targetUser) { targetSock = it->first; break; }
                }

                if (targetSock == INVALID_SOCKET) { send(client, "User not online\n", 16, 0); continue; }

                std::string privateMsg = "(Private) " + loggedIn[client] + ": " + message + "\n";
                send(targetSock, privateMsg.c_str(), (int)privateMsg.size(), 0);
                send(client, privateMsg.c_str(), (int)privateMsg.size(), 0);
            }
            // --- PUBLIC MESSAGE ---
            else {
                if (!loggedIn.count(client)) { send(client, "Login first\n", 12, 0); continue; }
                if (!userRoom.count(client)) { send(client, "Join room first\n", 17, 0); continue; }

                std::string roomName = userRoom[client];
                std::string fullMsg = loggedIn[client] + ": " + msg;
                broadcastRoom(fullMsg, roomName, client);
            }
        }
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}
