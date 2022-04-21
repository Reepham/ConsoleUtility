﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUtility
{
    public class Commands
    {

        private readonly Action<CancellationToken> methodeCanc;
        private readonly Action methode;
        private CancellationTokenSource tokensrc;

        public Commands(Action<CancellationToken> method)
        {
            this.methodeCanc = method;
        }
        public Commands(Action method)
        {
            this.methode = method;
        }
        
        public static void Start(Action<CancellationToken> testmethode)
        {
            new Commands(testmethode).CallCommands();
        }
        public static void Start(Action testmethode)
        {
            new Commands(testmethode).CallCommands();
        }

        private void CallCommands()
        {
            this.Help();

            bool repeat = true;
            while (repeat)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        this.Exit(ref repeat);
                        break;
                    case "help":
                        this.Help();
                        break;
                    case "repeat":
                        this.Repeat();
                        break;
                    case "break":
                        this.Break();
                        break;
                }
            }
        }
        
        private bool Exit(ref bool val)
        {
            return val=false;
        }

        private void Help()
        {
            Console.WriteLine(
                 "\n------------------------" +
                 "\n Commands:" +
                 "\n help - show a list of commands" +
                 "\n exit - close the console" +
                 "\n repeat - repeat the Test" +
                 "\n break - stop the Test during Execution (only for Tests with CancellationToken)" +
                 "\n------------------------");
        }

        private async void Repeat()
        {
            if (this.methode != null)
            {
                this.methode();
                return;
            }
            this.tokensrc = new CancellationTokenSource();
            CancellationToken token = tokensrc.Token;
            var t = Task.Run(() => {
                try
                {
                    this.methodeCanc(token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Abbruch des Tests!");
                }
                
                }
            , token);
            await t;
            // funzt, allerdings MemoryOverflow wenn man es zu oft wiederholt...
            CallCommands();

        }

        private void Break()
        {
            if (tokensrc != null)
            {
                tokensrc.Cancel();
            }
        }

    }
}
