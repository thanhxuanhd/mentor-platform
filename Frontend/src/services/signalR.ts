import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(
    `https://mentor-platform-gr4-be-bqbhg4b9a2e8dhbm.southeastasia-01.azurewebsites.net/message-hub`,
    {
      accessTokenFactory: () => localStorage.getItem("token") || "",
    },
  )
  .configureLogging(signalR.LogLevel.Information)
  .withAutomaticReconnect()
  .build();

export default connection;
