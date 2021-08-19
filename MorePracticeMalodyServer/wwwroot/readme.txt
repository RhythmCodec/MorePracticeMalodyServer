You must have wwwroot folder exists before start server.
Otherwise static file middleware won't map this folder after start.

启动服务器前需要确保 wwwroot 文件夹存在。
启动后手动添加次文件夹，中间件不会检测到更改，只有再次启动服务器后才会提供此文件夹中的文件。