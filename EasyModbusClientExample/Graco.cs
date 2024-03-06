using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyModbusClientExample
{
    public class Port
    {
        public string portName;
        public int portNumber;
        public int reg_read_state;
        public int reg_read_current_value;
        public int reg_read_target;
        public int reg_read_tolerance;

        public Port(string portName, int portNumber, int reg_read_state, int reg_read_current_value, int reg_read_target, int reg_read_tolerance)
        {
            this.portName = portName;
            this.portNumber = portNumber;
            this.reg_read_state = reg_read_state;
            this.reg_read_current_value = reg_read_current_value;
            this.reg_read_target = reg_read_target;
            this.reg_read_tolerance = reg_read_tolerance;
        }

        public int getPortNumber() { return portNumber; }

        public void setPortNumber(int num) { portNumber = num; }

        public string getPortName() { return portName; }

        public void setPortName(string name) { portName = name; }


    }
    
    public class Graco  // TODO: implements modbus interface
    {
        // Constant Registers
        const int reg_read_current_state = 100;
        const int reg_read_current_recipe = 102;
        const int reg_write_current_state = 402;
        const int reg_write_current_recipe = 400;

        // Ports
        Port port1;
        Port port2;
        Port port3;
        Port port4;
        Port port5;
        Port port6;
        Port port7;
        Port port8;

        // Modbus Client
        private EasyModbus.ModbusClient modbusClient;

        public Graco()
        {

            // All 8 ports and their registers for GRACO PRODISPENSE.
            // portName, PortNumber, stateReg, currentReg, targetReg, toleranceReg
            port1 = new Port("300PSI2", 1, 106, 154, 202, 218);
            port2 = new Port("HyTran", 2, 108, 156, 204, 220);
            port3 = new Port("300PSI", 3, 110, 158, 206, 222);
            port4 = new Port("DEF", 4, 112, 160, 208, 224);
            port5 = new Port("Synthetic", 5, 114, 162, 210, 226);
            port6 = new Port("Engine Oil", 6, 116, 164, 212, 228);
            port7 = new Port("Dye 2 - ML", 7, 118, 166, 214, 230);
            port8 = new Port("Dye", 8, 120, 168, 216, 232);

            // Client
            modbusClient = new EasyModbus.ModbusClient();
        }

        public bool Connect(string hostname, int port)
        {
            modbusClient.IPAddress = hostname;
            modbusClient.Port = port;
            modbusClient.Connect();

            return modbusClient.Connected;
        }

        public void disconnect()
        {
            modbusClient.Disconnect();
        }

        public bool setRecipe(int recipe)
        {
            // sets states and writes given recipe index to the fill machine
            modbusClient.WriteSingleRegister(reg_write_current_recipe, recipe);

            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {
                if(getRecipe() == recipe)
                {
                    return true;
                }

                if (stopwatch.ElapsedMilliseconds > 2000)
                {
                    return false;
                }
            }
        }


        

        public int getRecipe()
        {
            // returns current selected recipe
            int[] recipe_current = modbusClient.ReadHoldingRegisters(reg_read_current_recipe, 1);
            return recipe_current[0];
        }

        public void setSystemState(int state)
        {
            modbusClient.WriteSingleRegister(reg_write_current_state, state);
        }

        public int getSystemState()
        {
            int[] state = modbusClient.ReadHoldingRegisters(reg_read_current_state, 1);
            return state[0];
        }

        public int[] getPortState()
        {
            // Standby/Initialization of each panel
            int[] portStates = modbusClient.ReadHoldingRegisters(106, 16);
            int[] portStates_valid = removeOddRegisters(portStates);
            return portStates_valid;
        }

        public int[] getPortStatus()
        {
            // Flow/Valve/Dispense/Job status
            int[] portStatus = modbusClient.ReadHoldingRegisters(122, 16);
            int[] portStatus_valid = removeOddRegisters(portStatus);
            return portStatus_valid;
        }

        public int[] getPortEvent()
        {
            // Represents Error states for ports
            int[] portEvents = modbusClient.ReadHoldingRegisters(138, 16);
            int[] portEvents_valid = removeOddRegisters(portEvents);
            return portEvents_valid;
        }

        public int[] getCurrentVolume()
        {
            // Report current fill value for each port 
            // ex: 1250 = 12.50 cc
            int[] currentVolumes = modbusClient.ReadHoldingRegisters(154, 16);
            int[] currentVolumes_valid = removeOddRegisters(currentVolumes);
            return currentVolumes_valid;
        }

        public int[] getPreviousVolume()
        {
            // Report fill volume for last fill job
            int[] previousVolumes = modbusClient.ReadHoldingRegisters(170, 16);
            int[] previousVolumes_valid = removeOddRegisters(previousVolumes);
            return previousVolumes_valid;
        }

        public int[] getCurrentFlow()
        {
            // Flow in cc/min
            // Value has fixed-point value with the lower 10 digits being the val to the right of decimal.
            // To obtain int value, ignore the lowest 10 digits.
            int[] currentFlows = modbusClient.ReadHoldingRegisters(186, 16);
            int[] currentFlows_valid = removeOddRegisters(currentFlows);
            return currentFlows_valid;
        }

        public int[] getDispenseTargets()
        {
            // cc for each panel
            // ex: 1250 = 12.50 cc
            int[] dispenseTargets = modbusClient.ReadHoldingRegisters(202, 16);
            int[] dispenseTargets_valid = removeOddRegisters(dispenseTargets);
            return dispenseTargets_valid;
        }

        public int[] getDispenseTolerance()
        {
            // value in percentage
            // ex: 12 = 12%
            int[] dispenseTolerances = modbusClient.ReadHoldingRegisters(218, 16);
            int[] dispenseTolerances_valid = removeOddRegisters(dispenseTolerances);
            return dispenseTolerances_valid;
        }

        public int[] getGrandTotalVolume()
        {
            // val in cc
            int[] grandTotals = modbusClient.ReadHoldingRegisters(234, 16);
            int[] grandTotals_valid = removeOddRegisters(grandTotals);
            return grandTotals_valid;
        }

        public void setPanelEnable(int port, int enable)
        {
            // 0 = not enabled, 1 = enabled
            switch (port)
            {
                case 1:
                    modbusClient.WriteSingleRegister(410, enable);
                    break;
                case 2:
                    modbusClient.WriteSingleRegister(412, enable);
                    break;
                case 3:
                    modbusClient.WriteSingleRegister(414, enable);
                    break;
                case 4:
                    modbusClient.WriteSingleRegister(416, enable);
                    break;
                case 5:
                    modbusClient.WriteSingleRegister(418, enable);
                    break;
                case 6:
                    modbusClient.WriteSingleRegister(420, enable);
                    break;
                case 7:
                    modbusClient.WriteSingleRegister(422, enable);
                    break;
                case 8:
                    modbusClient.WriteSingleRegister(424, enable);
                    break;
                default:
                    Console.WriteLine("Port Index Not Valid!");
                    break;

            }
        }

        public void setPanelUnits(int port, int unit)
        {
            // 0 = cc
            // 1 = Liters
            // 2 = oz
            // 3 = gal
            switch (port)
            {
                case 1:
                    modbusClient.WriteSingleRegister(426, unit);
                    break;
                case 2:
                    modbusClient.WriteSingleRegister(428, unit);
                    break;
                case 3:
                    modbusClient.WriteSingleRegister(430, unit);
                    break;
                case 4:
                    modbusClient.WriteSingleRegister(432, unit);
                    break;
                case 5:
                    modbusClient.WriteSingleRegister(434, unit);
                    break;
                case 6:
                    modbusClient.WriteSingleRegister(436, unit);
                    break;
                case 7:
                    modbusClient.WriteSingleRegister(438, unit);
                    break;
                case 8:
                    modbusClient.WriteSingleRegister(440, unit);
                    break;
                default:
                    Console.WriteLine("Port Index Not Valid");
                    break;
            }
        }

        private int[] removeOddRegisters(int[] reg)
        {
            // The proDispense Graco only uses even numbered registers.
            // This function will remove invalid data from unused registers.
            
            int newSize = reg.Length / 2 + reg.Length % 2;
            int[] newArray = new int[newSize];

            for  (int i = 0,j = 0; i<reg.Length; i += 2, j++)
            {
                newArray[j] = reg[i];
            }
            return newArray;
        }
    }
}
