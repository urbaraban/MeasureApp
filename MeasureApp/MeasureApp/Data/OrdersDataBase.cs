using SureMeasure.Orders;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SureOrder.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SureMeasure.Data
{
    public class OrdersDataBase : INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public OrdersDataBase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(OrderDataItem).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(OrderDataItem)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        public Task<List<OrderDataItem>> GetItemsAsync => Database.Table<OrderDataItem>().ToListAsync();


        public Task<List<OrderDataItem>> GetItemsNotDoneAsync()
        {
            return Database.QueryAsync<OrderDataItem>("SELECT * FROM [OrderDataItem] WHERE [Done] = 0");
        }

        public Task<OrderDataItem> GetItemAsync(int id)
        {
            return Database.Table<OrderDataItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save and return primary key
        /// </summary>
        /// <param name="item">add object</param>
        /// <returns></returns>
        public Task<int> SaveItemAsync(Order item)
        {
            xmlrw.Write(item, Constants.NewOrderPath);

            if (item.ID != 0)
            {
                return Database.UpdateAsync(item.DataItem);
            }
            else
            {
                return Database.InsertAsync(item.DataItem);
            }
        }

        /// <summary>
        /// Delete and return primary key
        /// </summary>
        /// <param name="item">remove object</param>
        /// <returns></returns>
        public async Task<int> DeleteItemAsync(OrderDataItem item)
        {
            int index = await Database.DeleteAsync(item);
            OnPropertyChanged("GetItemAsync");
            return index;
        }
    }
}