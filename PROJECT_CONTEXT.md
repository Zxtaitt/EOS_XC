# PROJECT_CONTEXT

## AI Quick Summary
```yaml
project_name: EOS_XC
project_type: Windows hardware test upper-computer
primary_domain: EOS (Electrical Over Stress)
language: C#
framework: .NET Framework 4.8
ui: WinForms
solution_file: ElectricalOverStressDetection.sln
entrypoint: ElectricalOverStressDetection/Program.cs

what_it_is:
  - 一个面向测试工程师或产线操作员的 Windows 桌面检测软件
  - 一个通过本地 XML 配置驱动的硬件测试平台
  - 一个负责调度示波器和驱动板执行 EOS 检测流程的上位机

what_it_is_not:
  - 不是 Web 项目
  - 不是微服务
  - 不是数据库业务系统
  - 不是云端平台

main_projects:
  ElectricalOverStressDetection: WinForms UI and operator workflow
  ElectricalOverStressProcess: detection orchestration engine
  ElectricalOverStressData: XML models, enums, serialization helpers
  BoardDriver: board-specific driver implementations
  OscilloscopeDriver: VISA-based oscilloscope communication

config_files:
  communication: C:\EOSPlatformPlan\Communication\Communication.xml
  plans: C:\EOSPlatformPlan\EOSPlan\<PlanName>.xml

outputs:
  logs: D:\ElectricalOverStressDetectionLog\*.txt
  images_default: D:\ElectricalOverStressPicture

external_dependencies:
  - Windows
  - VISA / Ivi.Visa.Interop
  - Oscilloscope
  - Serial or TCP-connected driver boards
```

## 1. 项目一句话说明
`EOS_XC` 是一个运行在 Windows 上的 EOS 自动检测上位机。操作员通过 WinForms 界面加载本地通讯配置和测试计划，程序再去控制示波器与多块驱动板，按通道执行钳位、上电、下电、触发采样，并输出 `Pass / Fail / Skip`、日志和截图。

## 2. 业务定位
这个项目的核心价值不是数据管理，而是把原本依赖人工和仪器操作的 EOS 硬件测试流程，固化成一个可重复执行的软件流程。

典型场景：
- 测试工程师在实验室或产线执行 EOS 检测
- 根据不同板型选择不同测试计划
- 根据不同工位、层位、位置和板卡数量执行多通道检测
- 保存检测日志和示波器截图，作为检测记录或追溯依据

## 3. 这不是哪类项目
为避免后续 AI 理解偏差，这里明确说明：

- 它不是前后端分离系统
- 它没有 HTTP API 作为核心架构
- 它没有数据库作为主要数据源
- 它没有容器化、服务编排、消息队列等后端基础设施
- 它的核心是本地 GUI + 仪器通讯 + 板卡控制 + XML 配置

## 4. 技术栈与运行形态
- 语言：`C#`
- 框架：`.NET Framework 4.8`
- UI：`WinForms`
- 工程组织：Visual Studio `sln + csproj`
- 仪器通讯：`Ivi.Visa.Interop`
- 板卡通讯：`SerialPort` 和部分 `TcpClient`
- 配置存储：本地 `XML`
- 输出介质：本地日志文件、PNG 图片

这是一个典型的本地桌面程序，而不是部署在服务器上的后台程序。

## 5. 解决方案结构
### `ElectricalOverStressDetection/`
主界面和操作流程所在项目。

负责内容：
- 应用入口
- 开始/停止检测
- 打开通讯配置界面
- 打开计划配置界面
- 绑定层、位置、起止通道
- 显示日志
- 用颜色显示通道结果

### `ElectricalOverStressProcess/`
核心检测编排逻辑所在项目。

负责内容：
- 初始化示波器
- 根据板型创建对应驱动
- 按通道逐项执行检测动作
- 记录 `Pass / Fail / Skip`
- 触发 UI 的日志和颜色更新

### `ElectricalOverStressData/`
数据模型和工具类所在项目。

负责内容：
- `CommunicationInfo`
- `DataPlan`
- `ChannelItem`
- `DetectionResult`
- `BindInfo`
- 枚举定义
- XML 序列化与反序列化
- 全局路径和列表缓存

### `BoardDriver/`
驱动板抽象与具体实现所在项目。

负责内容：
- 统一接口 `IBoardDriver`
- 不同板型的 `BoardDriver_*` 实现
- 串口版和部分网络版驱动
- 板卡钳位、上电、下电、继电器控制等动作

### `OscilloscopeDriver/`
示波器驱动所在项目。

负责内容：
- 封装 VISA 资源打开与关闭
- 发送示波器命令
- 读取字符串、数字和块数据
- 抓取示波器 PNG 数据

## 6. 关键运行流程
系统的主流程可以理解为：

1. 启动 WinForms 主程序。
2. 初始化本地路径，如 `Communication` 和 `EOSPlan` 目录。
3. 用户点击开始检测。
4. 程序读取 `Communication.xml`。
5. 程序根据 `Communication.OtherItem.PlanName` 读取对应的计划 XML。
6. 用户选择层位、位置、开始通道和结束通道。
7. 创建 `Process` 实例。
8. `Process.Initialize()` 初始化示波器和板卡驱动。
9. 按通道执行检测：
   - 根据 `ChannelItem` 设置示波器
   - 设置板卡钳位
   - 上电
   - 下电
   - 查询示波器是否触发
   - 抓取并保存 PNG
   - 生成该通道检测结果
10. UI 根据结果把通道标成绿色、红色或跳过。
11. 日志写入界面，同时落盘到本地日志目录。

## 7. 配置驱动模型
这个系统是典型的“配置驱动”设计，重点不是把流程写死在代码里，而是通过 XML 控制大部分运行参数。

### 通讯配置
`CommunicationInfo` 表示通讯层配置，核心内容包括：
- `BoardSerialPort`：驱动板串口号或网络地址字符串
- `BoardNumber`：驱动板数量
- `OscilloscopeAaddress`：示波器 VISA 地址
- `PlanName`：当前使用的测试计划名
- `IamgePath`：截图保存目录
- `BoardItem`：各板卡位置、编号、地址映射

### 测试计划
`DataPlan` 表示测试方案，核心内容包括：
- `PlanName`
- `ChannelCount`
- `BoardType`
- `ChannelItem`

其中 `ChannelItem` 是每个通道的检测步骤列表，定义了：
- 电流源还是电压源
- 设置值
- 单次还是步进上电
- 步进数
- 示波器触发源
- 示波器单位模式
- 触发斜率
- 触发电平
- 通道量程
- 基础延时
- 时间刻度

## 8. 结果判定方式
从当前代码看，这个系统的判定逻辑更偏“流程执行型”，不是复杂算法型。

当前明显可见的判定行为：
- 通道没有配置计划时，结果为 `Skip`
- 执行过程异常时，结果为 `Fail`
- 示波器没有触发时，结果为 `Fail`
- 能正常抓图并完成流程时，结果为 `Pass`

因此当前仓库更像“自动测试执行平台 + 证据留存工具”，而不是“高级波形分析平台”。

## 9. 外部依赖
运行这个系统通常需要：
- Windows 环境
- .NET Framework 4.8
- 已安装或可用的 `VISA` 组件
- `Ivi.Visa.Interop.dll`
- 可通讯的示波器
- 可通讯的驱动板
- 本地 XML 配置目录

如果这些外部条件不满足，程序即使能编译，也未必能完成真实检测。

## 10. 仓库中的关键文件
新 AI 想快速理解项目时，建议优先看这些文件：

- `ElectricalOverStressDetection.sln`
- `ElectricalOverStressDetection/Program.cs`
- `ElectricalOverStressDetection/ElectricalOverStressDetection.cs`
- `ElectricalOverStressDetection/FrmCommunicationInfo.cs`
- `ElectricalOverStressDetection/FrmPlanSetting.cs`
- `ElectricalOverStressDetection/FrmBindInfo.cs`
- `ElectricalOverStressProcess/Process.cs`
- `BoardDriver/IBoardDriver.cs`
- `BoardDriver/BoardDriverEnum.cs`
- `OscilloscopeDriver/VisaComInstrument.cs`
- `ElectricalOverStressData/DataPlan.cs`
- `ElectricalOverStressData/CommunicationInfo.cs`
- `ElectricalOverStressData/ChannelItem.cs`
- `ElectricalOverStressData/DataTool.cs`
- `ElectricalOverStressData/DataEnum.cs`

## 11. 代码阅读建议
如果要定位不同问题，推荐按下面方式切入：

- 看 UI 行为：先看 `ElectricalOverStressDetection/`
- 看实际检测流程：先看 `ElectricalOverStressProcess/Process.cs`
- 看板型差异：先看 `BoardDriver/BoardDriverEnum.cs` 和各个 `BoardDriver_*`
- 看示波器交互：先看 `OscilloscopeDriver/VisaComInstrument.cs`
- 看配置和数据结构：先看 `ElectricalOverStressData/`

## 12. 对后续 AI 的重要提醒
后续 AI 在处理这个仓库时，应默认遵守以下理解：

- 不要把它当成 Web 或 API 项目来分析
- 不要假设存在数据库、ORM、REST、前端框架
- 不要假设配置来自环境变量或远程配置中心
- 优先考虑“本地 XML + 本地文件 + 硬件通讯”
- 修改数据模型时要注意 XML 序列化兼容性
- 新增板型时，通常需要同时改：
  - `BoardDriverEnum`
  - 新的 `BoardDriver_*` 类
  - 计划配置中的 `BoardType`
- 修改检测流程时，重点检查：
  - `Process.Initialize()`
  - `DetectionProcess()`
  - `BaseProcess()`
  - `OscilloscopeSet()`
- 修改路径逻辑时，注意当前代码存在硬编码目录

## 13. 当前仓库的已知局限
- 没有 `README`
- 没有自动化测试工程
- 没有完整部署说明
- 业务背景说明较少
- 一些硬件依赖无法仅从源码中完全确认
- 真实检测标准可能还依赖外部文档、设备手册或测试规范

## 14. 最短结论
如果只能用一句话描述这个项目：

`EOS_XC` 是一个基于 WinForms 的 EOS 自动检测上位机，通过 XML 配置驱动示波器和测试板卡，按通道执行硬件检测并输出结果、日志和截图。
