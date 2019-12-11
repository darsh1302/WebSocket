# WebSocket
WebSocket - Client / Server 


There is 2 main project for this application. Both of them work individually as well as too. 

Projects 
1) Realtime.MetroLine
	
	Client side application which does have HTML page with bootstap. Application will be able to send StopID to server to get estimaded arrival time for the Metro. 
	Client application is using websocket to keep connction open to communicate with client aplications. 
	Client will keep get updated every 2 mins. 

	Client can point to their own application to get desired output. In this scenario, Client will point to itself and open websocket connection with it. 


2) Realtime.Server.MetroLine

	Serverside application is designed so client will be able to point to server application for websocket connection. 
	Idea of having seprate serverside application will give ability scale or versioning without end application to get affected. 
	Also, it will give ability to generate dynamic middleware which will help to control Websocket count and lifecycleto manage.


