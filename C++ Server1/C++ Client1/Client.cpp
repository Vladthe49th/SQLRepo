#include <iostream>
#include <string>
#include <winsock2.h>
#pragma comment(lib, "ws2_32.lib")

int main() {
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(5555);
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    connect(sock, (sockaddr*)&serverAddr, sizeof(serverAddr));

    char buffer[1024];
    int bytesRead = recv(sock, buffer, sizeof(buffer), 0);
    if (bytesRead > 0) {
        std::cout << std::string(buffer, bytesRead);
    }

    fd_set readfds;
    while (true) {
        FD_ZERO(&readfds);
        FD_SET(sock, &readfds);
        FD_SET(0, &readfds);

        select(0, &readfds, NULL, NULL, NULL);

        if (FD_ISSET(0, &readfds)) {
            std::string msg;
            std::getline(std::cin, msg);
            send(sock, msg.c_str(), msg.size(), 0);
        }

        if (FD_ISSET(sock, &readfds)) {
            char buffer[1024];
            int bytesRead = recv(sock, buffer, sizeof(buffer), 0);
            if (bytesRead <= 0) {
                std::cout << "Server disconnected\n";
                break;
            }
            std::cout << std::string(buffer, bytesRead);
        }
    }

    closesocket(sock);
    WSACleanup();
    return 0;
}
