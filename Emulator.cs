using System.IO.Ports;

namespace Hexapod;
class CPort
{
    public static bool emulate = true;
    private SerialPort? port;
    public CPort(bool emulate)
    {
        try
        {
            if (emulate)
            {
                return;
            }
            var ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                Console.WriteLine(ports[i]);
            }


            port = new SerialPort();

            port.PortName = "/dev/ttyACM0";
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.Parity = System.IO.Ports.Parity.None;
            port.StopBits = System.IO.Ports.StopBits.One;
            port.ReadTimeout = 1000;
            port.WriteTimeout = 1000;
            port.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: невозможно открыть порт:" + e.ToString());
            return;
        }
    }
    public void Write(String data)
    {
        if (emulate)
        {
            File.AppendAllText("log.txt", data);
            Console.WriteLine(data);
        }
        else port?.Write(data);
    }
    public void Close()
    {
        Console.WriteLine("port is closing >>>>>>>>>>>>>>");
        if (!emulate && port != null && port.IsOpen)
        {
            port?.Close();
        }
    }
}