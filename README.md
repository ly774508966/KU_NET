# KU_NET
a simple NET(HTTP,SOCKET) &amp;&amp; UI Framwork for Unity

As a Client Developer,this is a simple thought and Make an attempt with it ^ ^.

this is based on UNITY 5.2.1 

##

##HTTP 下载(支持断点续传):

```csharp
HttpClient http= new HttpClient()
http.BeginDownLoadFileFlushToFile("http://118.192.69.207:8083/monster.apk",Application.persistentDataPath+"/mons.apk",
			 delegate(string arg1, float arg2, bool arg3)
			 {
				value = arg2;
				isDone = arg3;
				LogMgr.Log("Onprocess " +"  float  ="+ arg2.ToString()  +" isDone "+ arg3.ToString());
			});
			
http.StartConnect();

##HTTP 请求(post/get):
###unAuto start

```csharp
HttpClient http= new HttpClient()
http.BeginPost("http://xxx",
			 delegate(string arg1)
			 {

			});
http.AddField("---","---");		
http.StartConnect();

###Auto Start

```csharp
HttpClient http= new HttpClient()
http.BeginGet("http://xxx",
			 delegate(string arg1, float arg2, bool arg3)
			 {
				value = arg2;
				isDone = arg3;
				LogMgr.Log("Onprocess " +"  float  ="+ arg2.ToString()  +" isDone "+ arg3.ToString());
			},true);

##KThread
kthread 内部维护了一个simple thread pool。

```csharp
KThread.StartTask(testTh);

void testTh()
{

AsynchronousSocketListener.StartListening();
}

## buffer(auto serialize & deserialize)

```csharp
var buffer = new ByteBuffer();
//序列化
buffer += 3;
buffer += 4f;
buffer +="test";
buffer +=(short)7;

//反序列化
int intvalue =(int)buffer;
float floatvalue =(float)buffer;
string str =(string)buffer;
short shortvalue =(short)buffer;

ByteBuffer 目前不自动支持bool，因为buffer在序列化和反序列化struct的时候，内部的bool自动转为4byte,但是普通的bool，则为1byte，所以期望用户自己处理

## struct  && json 

可以使用 StructMessage  和jsonmessage
struct 可自己维护序列化，反序列化，json可以通过内置jsonfx进行。


## UI
based on MonoDelegateView ,BaseView
```csharp
protected override void Awake ()
{
	base.Awake ();
	Vector3 pos = m_view.pos;
	BaseView.Create<UGUIView> (this, new HideTrans (gameObject));
	m_view.pos = pos; 
	button.GetListener ().onPointerClick = ButtonClick;


}

public void ButtonClick (GameObject gobh, BaseEventData data)
{

	ContentManager.mIns.Push (MainViewType.U2View);

}

简单叙述一下，content 是作为中间数据类与contentmanger进行交互，负责view切换和状态维护，每个BaseView都是一个生命对象，通过base自动维护其生命周期，主要负责ui界面，效果方面，MonoDelegateView下可以执行逻辑，让view去播放动作
，同时可以通过 AutoAlignWithRectTrans&& autopos可以进行自动简易布局。^ ^

事实上，我们只需要一个MonoDelegateView，和一个BaseView，外加一个prefab，就足够了，AbstractTrans是为了自定义切换效果实现的抽象类。

^^暂时想起来的就这些了

## License
```csharp
The MIT License (MIT)

Copyright (c) 2015 cjsjy123

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.



