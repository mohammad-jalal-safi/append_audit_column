
using ConsoleApp.Services;
using System;
using System.Data;
using System.Linq;
using System.Speech.Recognition;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            Migration migration = new Migration();
     
            migration.AddColumn();
        }

    }
}
