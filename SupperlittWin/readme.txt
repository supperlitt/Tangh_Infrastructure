https://www.nuget.org/downloads

https://docs.microsoft.com/zh-cn/nuget/create-packages/creating-a-package


1.生成一个待填写的 nuspec文件
nuget spec

2.用生成的nuspec文件，和csporj生成 nupkg文件， nuget上面只需要提供nupkg文件即可。
nuget pack SupperlittTool.csproj


# 其他用法
nuget pack <project-name>.nuspec

nuget spec [<package-name>]  可以包含默认值
