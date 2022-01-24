# WpfWithCppInMmf
**this Code is Written in Visual Studio 2022**

### Simple Way to Code WPF With CPP DataContext Switching via MMF

Basic Concept shown as below image


![WPFWithCPPInMMF](https://user-images.githubusercontent.com/47770079/150726525-7541e8e5-79db-46ec-a1c9-f5f0a0de521b.png)

# 1. What is the purpose of this code?
More than decades, Windows Developer had Suffered alot about User Interface area because have restricted choice about it. 
Microsoft Windows Class(MFC) have used for more than 20 years as UI Application Program And now even MS abandoned of that.

late days, almost of windows developer uses C# in .NET Framework. it is standard solution now, but some case we have to use C/C++ code to build an Application
Using massive native code based Library Project.

C/C++ Code may Marshalled for using in C# but it Costs lots of time. 
So, This Sample shows VerySimple way to use C/C++ code with WPF(C#) context by Using Memory Mapped File. 

# 2. Check point of this project

* shared Structure

```cpp
struct SharedData
{
	int integerData;       //4 byte
	double doubleData;     //8 byte
	char stringData[256];  //256 byte
};
```

```C#
    public struct SharedData
    {
        public int integerData;                      //4 byte
        public double doubleData;                    //8 byte
        public byte stringData;                      //assume 256 byte, plus whole padding bytes
    }
```
- SharedData Structure in CPP, C# Must Treated As BYTES because its memory structure never can be same,
below link explains well
https://www.geeksforgeeks.org/structure-member-alignment-padding-and-data-packing/

* if you don't want to calculate data padding then just get sizeof(structure) in CPP code and assign bytes to C# code, that's easy and fast.

