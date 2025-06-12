using AlbionDataHandlers;
using AlbionDataHandlers.Enums;
using AlbionRadar.Managers;
using BaseUtils.Logger.Impl;
using PacketDotNet;
using SharpPcap;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlbionRadar
{
    /// <summary>  
    /// Main program class for AlbionRadar.  
    /// Handles packet capture and processing with configurable filtering.
    /// </summary>  
    public class Program
    {
        private readonly AlbionDataParser _albionDataParser;
        private readonly GameStateManager _gameStateManager;

        /// <summary>  
        /// Constructor to initialize the AlbionDataParser and GameStateManager.  
        /// </summary>  
        /// <param name="albionDataParser">Instance of AlbionDataParser.</param>  
        /// <param name="gameStateManager">Instance of GameStateManager.</param>  
        public Program(AlbionDataParser albionDataParser, GameStateManager gameStateManager)
        {
            _albionDataParser = albionDataParser;
            _gameStateManager = gameStateManager;
        }

        /// <summary>  
        /// Starts the application with device selection and filter configuration.
        /// </summary>  
        public void Start()
        {
            try
            {
                // Step 1: Select network device
                ICaptureDevice device = PacketDeviceSelector.AskForPacketDevice();
                if (device == null)
                {
                    DLog.E("No network device selected. Exiting.");
                    return;
                }

                // Step 2: Configure filter options
                TierLevels selectedTierFilter = ShowFilterDialog();

                // Step 3: Apply the selected filter to GameStateManager
                _gameStateManager.UpdateTierFilter(selectedTierFilter);

                DLog.I($"Starting AlbionRadar with tier filter: {selectedTierFilter}");

                // Step 4: Start packet capture
                StartPacketCapture(device);
            }
            catch (Exception ex)
            {
                DLog.E($"Error starting application: {ex}");
                MessageBox.Show($"Failed to start AlbionRadar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows the filter configuration dialog and returns the selected tier filter.
        /// </summary>
        /// <returns>The selected tier filter, or Tier5 as default.</returns>
        private TierLevels ShowFilterDialog()
        {
            using (var filterDialog = new FilterOptionDialog())
            {
                DialogResult result = filterDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    DLog.I($"User selected tier filter: {filterDialog.SelectedTierFilter}");
                    return filterDialog.SelectedTierFilter;
                }
                else
                {
                    DLog.I("User cancelled filter selection, using default Tier5");
                    return TierLevels.Tier5; // Default fallback
                }
            }
        }

        /// <summary>
        /// Starts the packet capture process on the selected device.
        /// </summary>
        /// <param name="device">The network capture device.</param>
        private void StartPacketCapture(ICaptureDevice device)
        {
            // Attach the packet handler to process incoming packets.  
            device.OnPacketArrival += PacketHandler;

            // Open the device in MaxResponsiveness mode with a read timeout of 1000ms.  
            device.Open(DeviceModes.MaxResponsiveness, 1000);

            // Start capturing packets.  
            device.StartCapture();

            DLog.I($"Packet capture started on device: {device.Description}");
        }

        /// <summary>  
        /// Handles incoming packets and processes UDP packets on port 5056.  
        /// </summary>  
        /// <param name="sender">The source of the event.</param>  
        /// <param name="e">Packet capture event arguments.</param>  
        private void PacketHandler(object sender, PacketCapture e)
        {
            try
            {
                // Extract the raw packet data.  
                RawCapture rawCapture = e.GetPacket();

                // Parse the packet and extract the UDP layer.  
                UdpPacket? packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data).Extract<UdpPacket>();

                // Check if the packet is a UDP packet and matches the target port (5056).  
                if (packet != null && (packet.SourcePort == 5056 || packet.DestinationPort == 5056))
                {
                    // Process the packet asynchronously to avoid blocking the main thread.  
                    Task.Run(() =>
                    {
                        try
                        {
                            // Pass the packet payload to the AlbionDataParser for processing.  
                            _albionDataParser.ReceivePacket(packet.PayloadData);
                        }
                        catch (Exception ex)
                        {
                            // Log any errors that occur during packet processing.  
                            DLog.E($"Error processing packet: {ex}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                DLog.E($"Error in packet handler: {ex}");
            }
        }

        // Note: Remove this Main method if you already have one elsewhere in your project
        // This was provided as an example - use your existing Main method instead
    }
}