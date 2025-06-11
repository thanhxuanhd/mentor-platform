import * as signalR from "@microsoft/signalr";

const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl(`https://localhost:5000/message-hub`, {
    accessTokenFactory: () => {
      return localStorage.getItem("token") || "";
    },
  })
  .withAutomaticReconnect()
  .build();

export default hubConnection;
