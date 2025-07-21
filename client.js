import WebSocket from 'ws';

const socket = new WebSocket('ws://localhost:3000');

socket.on('open', () => {
  console.log('서버에 연결되었습니다.');
  socket.send('클라이언트에서 보냄!');
});

socket.on('message', (data) => {
  console.log('서버로부터 메시지:', data.toString());
});

socket.on('close', () => {
  console.log('연결이 종료되었습니다.');
});
