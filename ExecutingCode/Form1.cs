using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ExecutingCode
{
    public partial class Form1 : Form
    {
        public static String filePath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            var originalConsoleOut = Console.Out; // preserve the original stream
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                try
                {
                    start();
                }
                catch {
                    Console.WriteLine("There was an error!");
                }

                writer.Flush(); // when you're done, make sure everything is written out

                var myString = writer.GetStringBuilder().ToString();
                richTextBox2.Text = myString;
            }
            Console.SetOut(originalConsoleOut); // restore Console.Out
        }

        public void start() {
            string source =
            @"
            using System;
            namespace Foo
            {
                public class Bar
                {
                    public void Main()
                    {" +
                       richTextBox1.Text + "}}}";

                Dictionary<string, string> providerOptions = new Dictionary<string, string>
                {
                    {"CompilerVersion", "v3.5"}
                };
                CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

                CompilerParameters compilerParams = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                };

                CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

                if (results.Errors.Count != 0)
                    throw new Exception("Mission failed!");

                object o = results.CompiledAssembly.CreateInstance("Foo.Bar");
                MethodInfo mi = o.GetType().GetMethod("Main");
                mi.Invoke(o, null);
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                sw.WriteLine(richTextBox1.Text);

            filePath = saveFileDialog1.FileName;
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            richTextBox1.Text = File.ReadAllText(openFileDialog1.FileName);
            filePath = openFileDialog1.FileName;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                    sw.WriteLine(richTextBox1.Text);
            }
            else {
                saveFileDialog1.ShowDialog();
            }
        }
    }
}
