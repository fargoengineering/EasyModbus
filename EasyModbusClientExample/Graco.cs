using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyModbusClientExample
{
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

        public Graco(string hostname)
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
            modbusClient.IPAddress = hostname;
        }

        void writeRecipe(int recipe)
        {
            // sets states and writes given recipe index to the fill machine
            modbusClient.WriteSingleRegister(reg_write_current_recipe, recipe);
        }

        int readRecipe()
        {
            // returns current selected recipe
            int[] recipe_current = modbusClient.ReadHoldingRegisters(reg_read_current_recipe, 1);
            return recipe_current[0];
        }

        void setSystemState(int state)
        {
            modbusClient.WriteSingleRegister(reg_write_current_state, state);
        }

        int getSystemState()
        {
            int[] state = modbusClient.ReadHoldingRegisters(reg_read_current_state,1);
            return state[0];
        }

        int[] getPortState()
        {
            // Standby/Initialization of each panel
            int[] portStates = modbusClient.ReadHoldingRegisters(106, 8);
            return portStates;
        }

        int[] getPortStatus()
        {
            // Flow/Valve/Dispense/Job status
            int[] portStatus = modbusClient.ReadHoldingRegisters(122,8);
            return portStatus;
        }

        int[] getPortEvent()
        {
            // Represents Error states for ports
            int[] portEvents = modbusClient.ReadHoldingRegisters(138, 8);
            return portEvents;
        }

        int[] getCurrentVolume()
        {
            // Report current fill value for each port 
            // ex: 1250 = 12.50 cc
            int[] currentVolumes = modbusClient.ReadHoldingRegisters(154, 8);
            return currentVolumes;
        }

        int[] getPreviousVolume()
        {
            // Report fill volume for last fill job
            int[] previousVolumes = modbusClient.ReadHoldingRegisters(170, 8);
            return previousVolumes;
        }

        int[] getCurrentFlow()
        {
            // Flow in cc/min
            // Value has fixed-point value with the lower 10 digits being the val to the right of decimal.
            // To obtain int value, ignore the lowest 10 digits.
            int[] currentFlows = modbusClient.ReadHoldingRegisters(186, 8);
            return currentFlows;
        }

        int[] getDispenseTargets()
        {
            // cc for each panel
            // ex: 1250 = 12.50 cc
            int[] dispenseTargets = modbusClient.ReadHoldingRegisters(202, 8);
            return dispenseTargets;
        }

        int[] getDispenseTolerance()
        {
            // value in percentage
            // ex: 12 = 12%
            int[] dispenseTolerances = modbusClient.ReadHoldingRegisters(218, 8);
            return dispenseTolerances;
        }

        int[] getGrandTotalVolume()
        {
            // val in cc
            int[] grandTotals = modbusClient.ReadHoldingRegisters(234, 8);
            return grandTotals;
        }

        void setPanelEnable(int port, int enable)
        {
            // 0 = not enabled, 1 = enabled
            switch(port)
            {
                case 1:
                    modbusClient.WriteSingleRegister(410,enable);
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

        void setPanelUnits(int port, int unit)
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
    }
}
