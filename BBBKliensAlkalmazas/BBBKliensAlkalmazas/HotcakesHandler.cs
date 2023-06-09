﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.Modules.Core.Admin.Configuration;

using BBBKliensAlkalmazas.DataModels;
using Hotcakes.CommerceDTO.v1;

namespace BBBKliensAlkalmazas
{


    internal class HotcakesStore
    {
        public static string[] OrderStatuses = { "Cancelled", "On Hold", "Received", "Ready for Payment", "Ready for Shipping", "Complete" };


        private const string URL = "http://20.234.113.211:8098/";
        private const string API_KEY = "1-635f3c22-ad1b-4c50-bd17-1286358ce37d";
        private Hotcakes.CommerceDTO.v1.Client.Api proxy;

        private List<Order> _orders = new List<Order>();
        private List<User> _users = new List<User>();

        public List<Order> Orders { get { return _orders; } }
        public List<User> Users { get { return _users; } }

        public void Init()
        /*
         Api kapcsolat létrehozésa. Szügséges az összes lekérdezéshez.
         Amennyiben nincs init, akkor egy "HotcakesStore must be initialized first!"
         hiba keletkezik.
         */
        {
            proxy = new Hotcakes.CommerceDTO.v1.Client.Api(URL, API_KEY);
        }

        private bool DownloadUsersFromHotcakes()
        {
            if (proxy == null)
            {
                throw new Exception("HotcakesStore must be initialized first!");
            }

            var snaps = proxy.CustomerAccountsFindAll();

            _users = new List<User>();
            if (snaps.Content != null)
            {
                for (int i = 0; i < snaps.Content.Count; i++)
                {
                    try
                    {
                        User user = new User();
                        var data = snaps.Content[i];

                        user.Bvin = data.Bvin.ToString();
                        user.FullName = data.FirstName + " " + data.LastName;
                        user.Email = data.Email;
                        _users.Add(user);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to convert at Content " + i + " exception: " + ex);
                    }
                }
                return true;
            }
            else
            {
                Debug.WriteLine("Empty response!");
                foreach (var error in snaps.Errors)
                {
                    Debug.WriteLine(error.Description);
                }
                return false;
            }
        }
        private bool DownloadOrdersFromHotcakes()
        {
            if (proxy == null)
            {
                throw new Exception("HotcakesStore must be initialized first!");
            }

            var snaps = proxy.OrdersFindAll();

            _orders = new List<Order>();
            if (snaps.Content != null)
            {
                for (int i = 0; i < snaps.Content.Count; i++)
                {
                    try
                    {
                        Order order = new Order();

                        order.Name = "NA";

                        foreach (var user in _users)
                        {
                            if (user.Email == snaps.Content[i].UserEmail)
                            {
                                order.Name = user.FullName;
                            }
                        }               
                        order.Email = snaps.Content[i].UserEmail;
                        order.Address = snaps.Content[i].ShippingAddress.CountryName + " " + snaps.Content[i].ShippingAddress.PostalCode +
                            " " + snaps.Content[i].ShippingAddress.City + " " + snaps.Content[i].ShippingAddress.Line1;
                        order.Date = snaps.Content[i].TimeOfOrderUtc;
                        order.Price = Convert.ToInt32(snaps.Content[i].TotalGrand);
                        order.Id = snaps.Content[i].Id;
                        order.Bvin = snaps.Content[i].bvin.ToString();
                        order.Status = snaps.Content[i].StatusName;
                        order.StatusCode = snaps.Content[i].StatusCode.ToString();
                        _orders.Add(order);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to convert Content " + i + " exception: " + ex);
                    }
                }
                return true;
            }
            else
            {
                Debug.WriteLine("Empty response!");
                foreach (var error in snaps.Errors)
                {
                    Debug.WriteLine(error.Description);
                }
                return false;
            }
        }

        public bool UpdateDataFromHotcakes()
        {
            if (DownloadUsersFromHotcakes())
            {
                return DownloadOrdersFromHotcakes();
            }
            return false;
        }

        public List<Item> GetOrderItems(string orderId)
        {
            if (proxy == null)
            {
                throw new Exception("HotcakesStore must be initialized first!");
            }

            var response = proxy.OrdersFind(orderId);

            if (response != null)
            {
                try
                {
                    var itemsHotcakes = response.Content.Items;

                    List<Item> items = new List<Item>();

                    for (int i = 0; i < itemsHotcakes.Count; i++)
                    {
                        Item item = new Item();
                        item.Name = itemsHotcakes[i].ProductName;
                        item.Id = itemsHotcakes[i].ProductId;
                        item.Price = itemsHotcakes[i].BasePricePerItem;
                        item.Quantity = itemsHotcakes[i].Quantity;
                        items.Add(item);
                    }
                    Debug.WriteLine(items);
                    return items;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("Empty Response:\n" + response.Errors[0].Description);
                return null;
            }
        }

        public bool SetOrderState(string orderId, string newStatus)
        {
            if (proxy == null)
            {
                throw new Exception("HotcakesStore must be initialized first!");
            }

            var response = proxy.OrdersFind(orderId);

            var order = response.Content;

            if (order == null)
            {
                foreach (var error in response.Errors)
                {
                    Debug.WriteLine(error.Description);
                }
                return false;
            }

            order.StatusName = newStatus;

            response = proxy.OrdersUpdate(order);

            if (response.Errors.Count > 0)
            {
                foreach (var error in response.Errors)
                {
                    Debug.WriteLine(error.Description);
                }
                return false;
            }
            return true;
        }
    }
}

