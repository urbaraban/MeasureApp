using System;
using System.Collections.Generic;

namespace SureMeasure.ShapeObj
{
    public class SheetMenu
    {
        public event EventHandler<string> ReturnValue;

        public List<SheetMenuItem> menuItems;

        public SheetMenu(List<SheetMenuItem> MenuItems)
        {
            this.menuItems = MenuItems;
        }

        public string[] ToArray()
        {
            List<string> temp = new List<string>();

            foreach (SheetMenuItem sheetMenuItem in menuItems)
            {
                temp.Add(sheetMenuItem.Name);
            }

            return temp.ToArray();
        }

        public async void ShowMenu(object sender, string Head)
        {
            string result = await AppShell.Instance.SheetMenuDialog(this, Head);

            foreach (SheetMenuItem sheetMenuItem in menuItems)
            {
                if (sheetMenuItem.Name == result)
                {
                    if (sheetMenuItem.Command.CanExecute(sender) == true)
                    {
                        sheetMenuItem.Command.Execute(sender);
                    }
                }
            }
        }
    }
}
