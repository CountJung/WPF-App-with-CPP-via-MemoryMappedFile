using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFAppCPPMMF
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        //MMF
        private readonly uint shareMemBufferSize = 272; // size of SharedData
        private Task? watchMMFTask;
        private MemoryMappedViewAccessor? accessorMMF;
        private CancellationTokenSource ctsForMMFTask;
        //Command
        public ViewCommander CmdOpenOrCreate { get; private set; }
        public ViewCommander CmdOpenCPPConsole { get; private set; }
        //control
        private string controlledText=$"This will be changed{Environment.NewLine}via console commands";
        public string ControlledText
        {
            get => controlledText;
            set
            {
                controlledText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("controlledText"));
            }
        }
        private Brush? textBlockBrush;
        public Brush? TextBlockBrush
        {
            get => textBlockBrush;
            set
            {
                textBlockBrush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("textBlockBrush"));
            }
        }
        // Note : Never can be same structure as CPP structure, Just Same Format
        public struct SharedData
        {
            public int integerData;                      //4 byte
            public double doubleData;                    //8 byte
            public byte stringData;                      //assume 256 byte, plus whole padding bytes
        }
        private SharedData shareDataWithCPP;

        public event PropertyChangedEventHandler? PropertyChanged;
        private object justLock;
        public MainViewModel()
        {
            shareDataWithCPP = new SharedData() { doubleData = 0, integerData = 0, stringData = 0 };
            watchMMFTask = null;
            accessorMMF = null;
            ctsForMMFTask = new CancellationTokenSource();
            justLock = new object();
            TextBlockBrush = (Brush) new BrushConverter().ConvertFromString("Beige");
            CmdOpenOrCreate = new ViewCommander(act => OpenOrCreateMMF());
            CmdOpenCPPConsole = new ViewCommander(act => OpenCPPConsole());
        }
        private void OpenCPPConsole()
        {
            // ### this button is just for test, if you want debug, use multi process debug or open 2 VS 
#if DEBUG
            Process.Start(@"..\..\..\..\x64\debug\cppconsolemmf.exe");
#else
            Process.Start(@"..\..\..\..\x64\release\cppconsolemmf.exe");
#endif
        }
        private void OpenOrCreateMMF()
        {
            if(watchMMFTask != null)
            {
                ctsForMMFTask.Cancel();
                watchMMFTask.Wait();
                ctsForMMFTask.Dispose();
                ctsForMMFTask = new CancellationTokenSource();
            }
            // this Class does not supported in .net5, .net6
            //MemoryMappedFileSecurity securityMMF = new MemoryMappedFileSecurity();
            MemoryMappedFile mapFile = MemoryMappedFile.CreateOrOpen("WPFWithCPPMMF", shareMemBufferSize, MemoryMappedFileAccess.ReadWrite);
            accessorMMF=mapFile.CreateViewAccessor(/*0, 268, MemoryMappedFileAccess.ReadWrite*/);
            CancellationToken token = ctsForMMFTask.Token;

            watchMMFTask = Task.Run(async () =>
              {
                  while (true)
                  {
                      if (token.IsCancellationRequested)
                          break;

                      lock (justLock)
                          accessorMMF.Read<SharedData>(0, out shareDataWithCPP);
                      
                      //Data Conversion
                      byte[] buffer = new byte[shareMemBufferSize];
                      accessorMMF.ReadArray(0, buffer, 0, buffer.Length);
                      int intData = BitConverter.ToInt32(buffer, 0);
                      double doubleData = BitConverter.ToDouble(buffer, 8);
                      string stringData = Encoding.ASCII.GetString(buffer, 16, 256);
                      //stringData=stringData.Replace("\0", string.Empty);
                      int zeroPos = stringData.IndexOf('\0');
                      stringData = stringData.Remove(zeroPos);

                      //Lamda has lamda :)
                      Dispatcher.CurrentDispatcher.Invoke(() =>
                      {
                          switch (intData)
                          {
                              case 0:
                                  TextBlockBrush = (Brush)new BrushConverter().ConvertFromString("Teal");
                                  break;
                              case 1:
                                  TextBlockBrush = (Brush)new BrushConverter().ConvertFromString("Orange");
                                  break;
                              case 2:
                                  TextBlockBrush = (Brush)new BrushConverter().ConvertFromString("Pink");
                                  break;
                              default:
                                  TextBlockBrush = (Brush)new BrushConverter().ConvertFromString("Beige");
                                  break;
                          }
                          ControlledText = string.Format($"TextData = {stringData}{Environment.NewLine}DoubleData = {doubleData}");
                      });
                      await Task.Delay(1);
                  }
              }/*, token*/);
        }
        public void CloseWindow()
        {
            ctsForMMFTask.Cancel();
            watchMMFTask?.Wait();
        }
    }
}
