﻿// ReSharper disable once CheckNamespace

namespace ExBuddy.OrderBotTags.Behaviors
{
    using Buddy.Coroutines;
    using Clio.XmlEngine;
    using ExBuddy.Windows;
    using ff14bot.Managers;
    using ff14bot.RemoteWindows;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    [XmlElement("EtxRetainer")]
    public class EtxRetainer : ExProfileBehavior
    {
        [DefaultValue(2)]
        [XmlAttribute("RetainerCount")]
        public int RetainerCount { get; set; }

        public new void Log(string text, params object[] args) { Logger.Info(text, args); }

        protected override async Task<bool> Main()
        {

            var retainerList = new RetainerList();

            foreach (var unit in GameObjectManager.GameObjects.OrderBy(r => r.Distance()))
                if (unit.NpcId == 2000401 || unit.NpcId == 2000441)
                {
                    unit.Interact();
                    break;
                }

            if (!await Coroutine.Wait(5000, () => RetainerList.IsOpen)) return isDone = true;
            {
                uint index = 0;
                while (index < RetainerCount)
                {
                    Log("Checking Retainer n° " + (index + 1));
                    await Coroutine.Sleep(200);
                    // Select retainer
                    await retainerList.SelectRetainerAndSkipDialog(index);
                    await Coroutine.Wait(5000, () => SelectString.IsOpen);
                    string ventureLine = SelectString.Lines()[5];
                    Log("Venture Status : " + ventureLine);
                    if (ventureLine.EndsWith("(Complete)") || ventureLine.EndsWith("Unternehmung einsehen") || ventureLine.EndsWith("tâche terminée") || ventureLine.EndsWith("[完了]") || ventureLine.EndsWith("[探险归来]"))
                    {
                        Log("Venture Completed !");
                        // Click on the completed venture
                        SelectString.ClickSlot(5);
                        await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen);
                        // Assign a new venture
                        RetainerTaskResult.Reassign();
                        await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                        // Confirm new venture
                        RetainerTaskAsk.Confirm();
                        await Coroutine.Wait(5000, () => Talk.DialogOpen);
                        // Skip Dialog
                        Talk.Next();
                        await Coroutine.Wait(5000, () => SelectString.IsOpen);
                    }
                    SelectString.ClickSlot((uint)SelectString.LineCount - 1);
                    await Coroutine.Wait(5000, () => Talk.DialogOpen);
                    // Skip Dialog
                    Talk.Next();
                    await Coroutine.Wait(5000, () => RetainerList.IsOpen);
                    index++;
                }
                Log("No more Retainer to check");
                await retainerList.CloseInstanceGently();
                return isDone = true;
            }
        }
    }
}