import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`https://localhost:5000/message-hub`, {
    accessTokenFactory: () => localStorage.getItem("token") || "",
  })
  .configureLogging(signalR.LogLevel.Information)
  .withAutomaticReconnect()
  .build();

export default connection;
