import { useState } from 'react'
import './App.css'
import Lobby from './components/Lobby'
import Chat from './components/Chat'
import {HubConnectionBuilder,LogLevel} from '@microsoft/signalR'
function App() {
  const [connection, setConnection] = useState()
  const [message, setMessage] = useState()
const joinRoom=async()=>{
  try {
    const connection=new HubConnectionBuilder().withUrl("https://localhost:7206/chatHub").configureLogging(LogLevel.Information)
    .build();
    connection.on("RecieveMessage", (user,Message)=>{
      setMessages(messages => [...messages, { user, message }]);
    })
    await connection.start()
    await connection.invoke("JoinRoom",{user,room})
    setConnection(connection);
    connection.on("UsersInRoom", (users) => {
      setUsers(users);
    });


    connection.onclose(e => {
      setConnection();
      setMessages([]);
      setUsers([]);
    });


  } catch (error) {
    console.log(error)
  }
}
const sendMessage = async (message) => {
  try {
    await connection.invoke("SendMessage", message);
  } catch (e) {
    console.log(e);
  }
}
const closeConnection = async () => {
  try {
    await connection.stop();
  } catch (e) {
    console.log(e);
  }
}

  return (
   <div className='App'>
<h2>My Chat</h2>
<hr className='line'/>  
{!connection
      ? <Lobby joinRoom={joinRoom} />
      : <Chat sendMessage={sendMessage} messages={messages} users={users} closeConnection={closeConnection} SendMessage={sendMessage}/>}
  </div>

  )
}

export default App
