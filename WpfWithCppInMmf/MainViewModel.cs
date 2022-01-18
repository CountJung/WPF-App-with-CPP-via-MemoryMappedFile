using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFAppCPPMMF
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        //MMF
        private readonly uint shareMemBufferSize = 268; // size of SharedData
        private Task? watchMMFTask;
        private MemoryMappedViewAccessor? accessorMMF;
        private CancellationTokenSource ctsForMMFTask;
        //Command
        public ViewCommander CmdOpenOrCreate { get; private set; }
        public struct SharedData
        {
            public int integerData;                      //4 byte
            public double doubleData;                    //8 byte
            public byte stringData;                      //assume 256 byte
        }
        private SharedData shareDataWithCPP;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            shareDataWithCPP = new SharedData();
            watchMMFTask = null;
            accessorMMF = null;
            ctsForMMFTask = new CancellationTokenSource();
            CmdOpenOrCreate = new ViewCommander(act => OpenOrCreateMMF());
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
            accessorMMF=mapFile.CreateViewAccessor(0, 268, MemoryMappedFileAccess.ReadWrite);
            CancellationToken token = ctsForMMFTask.Token;

            watchMMFTask = Task.Run(async () =>
              {
                  while (true)
                  {
                      if (token.IsCancellationRequested)
                          break;

                      accessorMMF.Read<SharedData>(0, out shareDataWithCPP);
                      //Data Conversion
                      byte[] buffer = new byte[268];
                      accessorMMF.ReadArray(0, buffer, 0, buffer.Length);
                      int intData=BitConverter.ToInt32(buffer, 0);
                      double doubleData = BitConverter.ToDouble(buffer, 4);
                      string stringData = Encoding.ASCII.GetString(buffer, 12, 256);
                      stringData=stringData.Replace("\0", string.Empty);

                      await Task.Delay(1);
                  }
              }, token);
        }
        public void CloseWindow()
        {
            ctsForMMFTask.Cancel();
            watchMMFTask?.Wait();
        }
    }
}
