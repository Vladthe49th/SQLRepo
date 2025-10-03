#include <iostream>
#include <string>
#include <map>
#include <vector>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

// --- Глобальні змінні ---
std::map<std::string, std::string> users;       // username -> password
std::map<SOCKET, std::string> loggedIn;         // socket -> username
std::vector<SOCKET> clients;

std::map<std::string, std::vector<SOCKET>> rooms; // roomname -> список сокетів
std::map<SOCKET, std::string> userRoom;          // клієнт -> кімната

const std::string USERS_FILE = "users.txt";

// --- Завантаження користувачів із файлу ---
void loadUsers() {
    std::ifstream fin(USERS_FILE);
    if (!fin.is_open()) return;
    std::string user, pass;
    while (fin >> user >> pass) {
        users[user] = pass;
    }
    fin.close();
    std::cout << "Loaded " << users.size() << " users from file.\n";
}

// --- Додавання нового користувача у файл ---
void saveUser(const std::string& user, const std::string& pass) {
    std::ofstream fout(USERS_FILE, std::ios::app);
    fout << user << " " << pass << "\n";
    fout.close();
}

// --- Розсилка повідомлення всім клієнтам (глобальний чат, якщо потрібно) ---
void broadcast(const std::string& message, SOCKET sender) {
    for (SOCKET client : clients) {
        if (client != sender) {
            send(client, message.c_str(), (int)message.size(), 0);
        }
    }
}

int main() {
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(5555);

    bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr));
    listen(serverSocket, 5);

    std::cout << "Server started. Waiting for connections...\n";

    loadUsers();

    fd_set readfds;

    while (true) {
        FD_ZERO(&readfds);
        FD_SET(serverSocket, &readfds);

        for (SOCKET client : clients) {
            FD_SET(client, &readfds);
        }

        int activity = select(0, &readfds, NULL, NULL, NULL);
        if (activity == SOCKET_ERROR) {
            std::cerr << "Select error\n";
            break;
        }

        // нове підключення
        if (FD_ISSET(serverSocket, &readfds)) {
            SOCKET newClient = accept(serverSocket, NULL, NULL);
            clients.push_back(newClient);
            std::cout << "New client connected\n";
            send(newClient, "Welcome! Please REGISTER or LOGIN\n", 36, 0);
        }

        // обробка повідомлень від клієнтів
        for (size_t i = 0; i < clients.size(); i++) {
            SOCKET client = clients[i];
            if (FD_ISSET(client, &readfds)) {
                char buffer[1024] = { 0 };
                int bytesRead = recv(client, buffer, sizeof(buffer), 0);
                if (bytesRead <= 0) {
                    std::cout << "Client disconnected\n";
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
                }
                else {
                    std::string msg(buffer, bytesRead);
                    std::cout << "Received: " << msg << "\n";

                    // --- REGISTER ---
                    if (msg.rfind("REGISTER", 0) == 0) {
                        char u[256], p[256];
                        if (sscanf(msg.c_str(), "REGISTER %s %s", u, p) == 2) {
                            std::string user = u, pass = p;
                            if (users.count(user)) {
                                send(client, "User already exists\n", 20, 0);
                            }
                            else {
                                users[user] = pass;
                                saveUser(user, pass);
                                send(client, "Registered successfully\n", 24, 0);
                            }
                        }
                        else {
                            send(client, "Usage: REGISTER <username> <password>\n", 38, 0);
                        }
                    }
                    // --- LOGIN ---
                    else if (msg.rfind("LOGIN", 0) == 0) {
                        char u[256], p[256];
                        if (sscanf(msg.c_str(), "LOGIN %s %s", u, p) == 2) {
                            std::string user = u, pass = p;
                            if (users.count(user) && users[user] == pass) {
                                loggedIn[client] = user;
                                send(client, "Login successful\n", 17, 0);
                            }
                            else {
                                send(client, "Invalid login\n", 14, 0);
                            }
                        }
                        else {
                            send(client, "Usage: LOGIN <username> <password>\n", 35, 0);
                        }
                    }
                    // --- PRIVATE MESSAGE ---
                    else if (msg.rfind("/w", 0) == 0 || msg.rfind("/msg", 0) == 0) {
                        if (!loggedIn.count(client)) {
                            send(client, "You must LOGIN first\n", 22, 0);
                            continue;
                        }

                        std::istringstream iss(msg);
                        std::string cmd, targetUser;
                        if (!(iss >> cmd >> targetUser)) {
                            send(client, "Usage: /w <username> <message>\n", 31, 0);
                            continue;
                        }

                        std::string message;
                        std::getline(iss, message);
                        if (!message.empty() && message.front() == ' ')
                            message.erase(0, 1);

                        if (message.empty()) {
                            send(client, "Usage: /w <username> <message>\n", 31, 0);
                            continue;
                        }

                        SOCKET targetSocket = INVALID_SOCKET;
                        for (auto it = loggedIn.begin(); it != loggedIn.end(); ++it) {
                            if (it->second == targetUser) {
                                targetSocket = it->first;
                                break;
                            }
                        }

                        if (targetSocket == INVALID_SOCKET) {
                            send(client, "User not online\n", 16, 0);
                            continue;
                        }

                        std::string fromUser = loggedIn[client];
                        std::string privateMsg = "(Private) " + fromUser + ": " + message + "\n";
                        send(targetSocket, privateMsg.c_str(), (int)privateMsg.size(), 0);
                        send(client, privateMsg.c_str(), (int)privateMsg.size(), 0);
                    }
                    // --- JOIN ROOM ---
                    else if (msg.rfind("/join", 0) == 0) {
                        if (!loggedIn.count(client)) {
                            send(client, "You must LOGIN first\n", 22, 0);
                            continue;
                        }

                        std::istringstream iss(msg);
                        std::string cmd, roomName;
                        if (!(iss >> cmd >> roomName)) {
                            send(client, "Usage: /join <roomname>\n", 25, 0);
                            continue;
                        }

                        if (userRoom.count(client)) {
                            std::string oldRoom = userRoom[client];
                            auto& vec = rooms[oldRoom];
                            vec.erase(std::remove(vec.begin(), vec.end(), client), vec.end());
                        }

                        rooms[roomName].push_back(client);
                        userRoom[client] = roomName;

                        std::string msgJoin = "You joined room: " + roomName + "\n";
                        send(client, msgJoin.c_str(), (int)msgJoin.size(), 0);
                    }
                    // --- LEAVE ROOM ---
                    else if (msg.rfind("/leave", 0) == 0) {
                        if (userRoom.count(client)) {
                            std::string roomName = userRoom[client];
                            auto& vec = rooms[roomName];
                            vec.erase(std::remove(vec.begin(), vec.end(), client), vec.end());
                            userRoom.erase(client);
                            send(client, "You left the room\n", 18, 0);
                        }
                        else {
                            send(client, "You are not in any room\n", 25, 0);
                        }
                    }
                    // --- LIST ROOMS ---
                    else if (msg.rfind("/rooms", 0) == 0) {
                        std::string list = "Rooms:\n";
                        for (auto& r : rooms) {
                            list += "- " + r.first + " (" + std::to_string(r.second.size()) + ")\n";
                        }
                        send(client, list.c_str(), (int)list.size(), 0);
                    }
                    // --- PUBLIC MESSAGE IN ROOM ---
                    else {
                        if (loggedIn.count(client)) {
                            if (!userRoom.count(client)) {
                                send(client, "You must join a room first (/join <roomname>)\n", 48, 0);
                                continue;
                            }

                            std::string roomName = userRoom[client];
                            std::string user = loggedIn[client];
                            std::string fullMsg = user + ": " + msg;

                            for (SOCKET sock : rooms[roomName]) {
                                if (sock != client) send(sock, fullMsg.c_str(), (int)fullMsg.size(), 0);
                            }
                        }
                        else {
                            send(client, "You must LOGIN first\n", 22, 0);
                        }
                    }
                }
            }
        }
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}
