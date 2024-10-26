## TCP
服务端 
ServerSocket：等待客户端连接，连接后创建ClientSocket  
ClientSocket：接受并处理客户端信息，发送信息

客户端  
ClientSocket  

公共  
Messager：发送和接收字节数组的逻辑，在头部添加长度和消息类型，解决黏包分包问题  
BaseMessage：基础消息类型，实现与字节数组的相互转换  