import * as signalR from "@microsoft/signalr";

const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl(`https://localhost:5000/message-hub`)
  .withAutomaticReconnect()
  .build();

export default hubConnection;
