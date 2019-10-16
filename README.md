## ForceFramework
+ 结构介绍：
	+ App->具体项目
	+ Common->公用工具类
	+ DataLayer->数据库操作类
	+ Model->数据库实体类，其他实体类
其中DataLayer和Model类通过生成工具生成，生成工具https://github.com/curiousPeng/CodeGenerator

#### 分支
+ master 是API接口。
+ MVC 是带视图的，带有基础的权限验证模块。

#### 数据库
+ 支持MySQL和SQL server，理论上CodeGenerator支持生成那种就可以用那种。
+ 项目地址：https://github.com/curiousPeng/CodeGenerator

#### 缓存
+ MemoryCache,已注入直接可用。
+ Redis,已注入直接可用，Redis进行了小封装，支持对象的直接hashset。

#### orm
+ CodeGenerator生成的是用Dapper,所以用Dapper。

#### 日志
+ NLog

#### 队列
+ RabbitMQ，封装了一下使用的是 * https://github.com/curiousPeng/Tools/tree/master/LightMessager * 项目。

#### 文档
+ Swagger

#### 异常
+ 使用StackExchange.Excenptional,默认是用的SQL server数据库，其他数据库使用，更改nuget包为其他版，原项目地址：https://github.com/NickCraver/StackExchange.Exceptional ，项目没有把Opserver放进去，
+ 配合https://github.com/opserver/Opserver 食用更佳

