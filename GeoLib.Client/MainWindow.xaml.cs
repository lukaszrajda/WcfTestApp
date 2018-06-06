using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GeoLib.Client
{
    public partial class MainWindow : Window
    {
        GeoClient _proxy = null;
        StatefulGeoClient _proxy2 = null;
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "UI Running on Thread " + Thread.CurrentThread.ManagedThreadId +
                " | Process " + Process.GetCurrentProcess().Id.ToString();
            _proxy = new GeoClient();
            _proxy.Open();
            _proxy2 = new StatefulGeoClient();
        }

        private void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "")
            {
                //GeoClient proxy = new GeoClient("tcpEP");
                string zipCode = txtZipCode.Text;
                try
                {
                    ZipCodeData data = _proxy.GetZipInfo(zipCode);
                    if (data != null)
                    {
                        lblCity.Content = data.City;
                        lblState.Content = data.State;
                    }
                }
                catch(FaultException<ExceptionDetail> ex)
                {
                    MessageBox.Show($"Exception thrown by service.\n\rException type :" +
                        $"FaultException<ExceptionDetail>\n\r" +
                        $"Message: {ex.Detail.Message}\n\r" +
                        $"Proxy state: {_proxy.State}");
                }
                catch (FaultException<ApplicationException> ex)
                {
                    MessageBox.Show($"FaultException<ApplicationException> thrown by service.\n\rException type :" +
                        $"FaultException<ApplicationException>\n\r" +
                        $"Message: {ex.Detail.Message}\n\r" +
                        $"Proxy state: {_proxy.State}");
                }
                catch (FaultException ex)
                {
                    MessageBox.Show($"FaultException thrown by service.\n\rException type :" +
                        $"{ex.GetType().Name}\n\r" +
                        $"Message: {ex.Message}\n\r" +
                        $"Proxy state: {_proxy.State}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Exception thrown by service.\n\rException type :" +
                        $"{ex.GetType().Name}\n\r" +
                        $"Message: {ex.Message}\n\r" +
                        $"Proxy state: {_proxy.State}");
                }
                //proxy.Close();
            }
        }

        private void btnGetZipCodes_Click(object sender, RoutedEventArgs e)
        {
            if (txtState.Text != null)
            {
                //EndpointAddress address = new EndpointAddress("net.tcp://localhost:8009/GeoService");
                //Binding binding = new NetTcpBinding();
                //
                //GeoClient proxy = new GeoClient(binding, address);
                //GeoClient proxy = new GeoClient(binding, address);
                GeoClient proxy = new GeoClient("tcpEP");
                IEnumerable<ZipCodeData> data = proxy.GetZips(txtState.Text);

                if (data != null)
                {
                    lstZips.ItemsSource = data;
                }
                proxy.Close();
            }
        }

        private void btnMakeCall_Click(object sender, RoutedEventArgs e)
        {
            Binding binding = new NetNamedPipeBinding();
            EndpointAddress address = new EndpointAddress("net.pipe://localhost/MessageService");

            //ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");
            ChannelFactory<IMessageService> factory = 
                new ChannelFactory<IMessageService>(binding, address);
            IMessageService proxy = factory.CreateChannel();
            proxy.ShowMsg(txtMessage.Text);
            factory.Close();
        }

        private void btnPush_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode != null)
            {
                _proxy2.PushZip(txtZipCode.Text);
            }
        }

        private void btnGetInRange_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "" && txtRange.Text != "")
            {
                IEnumerable<ZipCodeData> data = _proxy2.GetZips(int.Parse(txtRange.Text));

                if (data != null)
                {
                    lstZips.ItemsSource = data;
                }
            }
        }
    }
}
