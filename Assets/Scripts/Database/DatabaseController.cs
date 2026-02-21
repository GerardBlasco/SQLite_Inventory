using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using UnityEngine;

public class DataBaseController : MonoBehaviour
{
    public static DataBaseController instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    private static IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:" + Application.dataPath + "/GameDataBase.sqlite";
        
        // Creació de la connexió
        IDbConnection dbConnection = new SqliteConnection(dbUri);

        // Opertura de la connexió
        dbConnection.Open();

        IDbCommand dbCommandForeignKeys = dbConnection.CreateCommand();
        dbCommandForeignKeys.CommandText = "PRAGMA FOREIGN_KEYS = ON;";
        dbCommandForeignKeys.ExecuteNonQuery();
        dbCommandForeignKeys.Dispose();

        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();

        dbCommandCreateTable.CommandText = 
            "CREATE TABLE IF NOT EXISTS USERS " +
                "(userID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "username VARCHAR(20) UNIQUE NOT NULL, " +
                "password VARCHAR(30) NOT NULL CHECK (length(password) >= 8)," +
                "skinID INTEGER NOT NULL," +
                "FOREIGN KEY (skinID) REFERENCES SKIN (skinID))";

        dbCommandCreateTable.ExecuteReader();

        dbCommandCreateTable.Dispose();

        dbCommandCreateTable.CommandText = 
            "CREATE TABLE IF NOT EXISTS INVENTORY " +
                "(inventoryID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "userID INTEGER UNIQUE NOT NULL, " +
                "maxSlots INTEGER DEFAULT 15, " +
                "FOREIGN KEY (userID) REFERENCES USERS (userID) ON DELETE CASCADE)";

        dbCommandCreateTable.ExecuteReader();

        dbCommandCreateTable.Dispose();

        dbCommandCreateTable.CommandText = 
            "CREATE TABLE IF NOT EXISTS ITEM " +
                "(itemID VARCHAR(20) PRIMARY KEY, " +
                "description VARCHAR(80))";

        dbCommandCreateTable.ExecuteReader();

        dbCommandCreateTable.Dispose();

        dbCommandCreateTable.CommandText =
            "CREATE TABLE IF NOT EXISTS INVENTORY_SLOT " +
                "(inventoryID INTEGER, " +
                "itemID VARCHAR(20), " +
                "quantity INTEGER NOT NULL, " +
                "PRIMARY KEY (inventoryID, itemID), " +
                "FOREIGN KEY (inventoryID) REFERENCES INVENTORY (inventoryID), " +
                "FOREIGN KEY (itemID) REFERENCES ITEM (itemID))";

        dbCommandCreateTable.ExecuteReader();

        dbCommandCreateTable.Dispose();

        dbCommandCreateTable.CommandText =
            "CREATE TABLE IF NOT EXISTS SKIN " +
                "(skinID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "eyeID VARCHAR(20), " +
                "color CHAR(6), " +
                "UNIQUE (eyeID, color))";

        dbCommandCreateTable.ExecuteReader();

        dbCommandCreateTable.Dispose();

        return dbConnection;
    }

    public static void InsertNewUserIntoDB(string username, string password, string eyeID, string colorHEX)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();

        int newUserID = -1;
        int skinID = -1;

        IDbCommand dbCommandCheckSkin = dbConnection.CreateCommand();
        dbCommandCheckSkin.CommandText = $"SELECT skinID FROM SKIN WHERE eyeID = '{eyeID}' AND color = '{colorHEX}'";
        IDataReader skinReader = dbCommandCheckSkin.ExecuteReader();

        if (skinReader.Read())
        {
            skinID = skinReader.GetInt32(0);
        }

        skinReader.Close();
        dbCommandCheckSkin.Dispose();

        if (skinID == -1)
        {
            IDbCommand dbCommandInsertSkin = dbConnection.CreateCommand();
            dbCommandInsertSkin.CommandText = $"INSERT INTO SKIN (eyeID, color) VALUES ('{eyeID}', '{colorHEX}')";
            dbCommandInsertSkin.ExecuteNonQuery();
            dbCommandInsertSkin.Dispose();

            IDbCommand dbCommandSelectSkinID = dbConnection.CreateCommand();
            dbCommandSelectSkinID.CommandText = "SELECT last_insert_rowid()";
            IDataReader newSkinReader = dbCommandSelectSkinID.ExecuteReader();
            if (newSkinReader.Read())
            {
                skinID = newSkinReader.GetInt32(0);
            }
            newSkinReader.Close();
            dbCommandSelectSkinID.Dispose();
        }

        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = $"INSERT INTO USERS VALUES (NULL, '{username}', '{password}', {skinID})";
        dbCommandInsertValue.ExecuteNonQuery();
        dbCommandInsertValue.Dispose();

        IDbCommand dbCommandSelectID = dbConnection.CreateCommand();
        dbCommandSelectID.CommandText = "SELECT last_insert_rowid()";
        IDataReader idReader = dbCommandSelectID.ExecuteReader();

        if (idReader.Read())
        {
            newUserID = idReader.GetInt32(0);
        }

        idReader.Close();
        dbCommandSelectID.Dispose();

        IDbCommand dbCommandInsertInventory = dbConnection.CreateCommand();
        dbCommandInsertInventory.CommandText = $"INSERT INTO INVENTORY (userID) VALUES ({newUserID})";
        dbCommandInsertInventory.ExecuteNonQuery();
        dbCommandInsertInventory.Dispose();

        dbConnection.Close();
    }

    private static int GetInventoryID(IDbConnection connection, int userID)
    {
        int id = -1;

        IDbCommand dbCommandGetInventory = connection.CreateCommand();
        dbCommandGetInventory.CommandText = $"SELECT inventoryID FROM INVENTORY WHERE userID = {userID}";
        IDataReader reader = dbCommandGetInventory.ExecuteReader();

        if (reader.Read())
        {
            id = reader.GetInt32(0);
        }

        reader.Close();
        dbCommandGetInventory.Dispose();

        return id;
    }

    private static int GetUserID(IDbConnection connection)
    {
        int id = -1;

        IDbCommand dbCommandGetInventory = connection.CreateCommand();
        dbCommandGetInventory.CommandText = $"SELECT userID FROM USERS WHERE username = '{PlayerPrefs.GetString("LoggedUser")}'";
        IDataReader reader = dbCommandGetInventory.ExecuteReader();

        if (reader.Read())
        {
            id = reader.GetInt32(0);
        }

        reader.Close();
        dbCommandGetInventory.Dispose();

        return id;
    }

    private static int GetSkinID(IDbConnection connection, int userID)
    {
        int id = -1;

        IDbCommand dbCommandGetSkin = connection.CreateCommand();
        dbCommandGetSkin.CommandText = $"SELECT skinID FROM USERS WHERE userID = {userID}";
        IDataReader reader = dbCommandGetSkin.ExecuteReader();

        if (reader.Read())
        {
            id = reader.GetInt32(0);
        }

        reader.Close();
        dbCommandGetSkin.Dispose();

        return id;
    }

    public static void SaveInventoryIntoDatabase(List<InventorySlot> inventory) 
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();

        int userID = GetUserID(dbConnection);

        int inventoryID = -1;

        inventoryID = GetInventoryID(dbConnection, userID);

        if (inventoryID == -1)
        {
            //Debug.LogError("No se ha encontrado el inventario del usuario");
            return;
        }

        IDbCommand dbCommandReset = dbConnection.CreateCommand();
        dbCommandReset.CommandText = $"UPDATE INVENTORY_SLOT SET quantity = 0 WHERE inventoryID = {inventoryID}";
        dbCommandReset.ExecuteNonQuery();
        dbCommandReset.Dispose();

        foreach (InventorySlot s in inventory)
        {
            string itemID = s.GetItem().GetID();
            int quantity = s.GetQuantity();

            IDbCommand dbCommandCheckItem = dbConnection.CreateCommand();
            dbCommandCheckItem.CommandText = $"SELECT itemID FROM ITEM WHERE itemID = '{itemID}'";
            IDataReader itemReader = dbCommandCheckItem.ExecuteReader();

            bool itemExists = itemReader.Read();
            itemReader.Close();
            dbCommandCheckItem.Dispose();

            if (!itemExists)
            {
                Debug.Log($"El item {itemID} no existe en la tabla ITEM");
            }

            IDbCommand dbCommandCheckSlot = dbConnection.CreateCommand();
            dbCommandCheckSlot.CommandText = $"SELECT quantity FROM INVENTORY_SLOT WHERE inventoryID = {inventoryID} AND itemID = '{itemID}'";
            IDataReader slotReader = dbCommandCheckSlot.ExecuteReader();

            if (slotReader.Read())
            {
                IDbCommand dbCommandUpdate = dbConnection.CreateCommand();
                dbCommandUpdate.CommandText = $"UPDATE INVENTORY_SLOT SET quantity = quantity + {quantity} WHERE inventoryID = {inventoryID} AND itemID = '{itemID}'";
                dbCommandUpdate.ExecuteNonQuery();
                dbCommandUpdate.Dispose();
            }
            else
            {
                IDbCommand dbCommandInsert = dbConnection.CreateCommand();
                dbCommandInsert.CommandText = $"INSERT INTO INVENTORY_SLOT (inventoryID, itemID, quantity) VALUES ({inventoryID}, '{itemID}', {quantity})";
                dbCommandInsert.ExecuteNonQuery();
                dbCommandInsert.Dispose();
            }

            slotReader.Close();
            dbCommandCheckSlot.Dispose();
        }

        IDbCommand dbCommandDelete = dbConnection.CreateCommand();
        dbCommandDelete.CommandText = $"DELETE FROM INVENTORY_SLOT WHERE inventoryID = {inventoryID} AND quantity <= 0";
        dbCommandDelete.ExecuteNonQuery();
        dbCommandDelete.Dispose();

        dbConnection.Close();
    }

    public static void LoadDataIntoPlayerConfig(PlayerConfig player)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();

        int userID = GetUserID(dbConnection);
        int skinID = GetSkinID(dbConnection, userID);

        IDbCommand dbCommandSelect = dbConnection.CreateCommand();
        dbCommandSelect.CommandText = $"SELECT eyeID, color FROM SKIN WHERE skinID = {skinID}";
        IDataReader dataReader = dbCommandSelect.ExecuteReader();

        if (dataReader.Read())
        {
            string eyeID = dataReader.GetString(0);
            string color = dataReader.GetString(1);

            player.SetColor(color);
            player.SetEyes(eyeID);
        }

        dataReader.Close();
        dbCommandSelect.Dispose();

        dbConnection.Close();
    }

    public static void LoadDatabaseIntoInventory(InventoryController inventory)
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();

        int userID = GetUserID(dbConnection);
        int inventoryID = -1;

        inventoryID = GetInventoryID(dbConnection, userID);

        if (inventoryID == -1)
        {
            //Debug.LogError("No se ha encontrado ningun inventario.");
            return;
        }

        IDbCommand dbCommandSelect = dbConnection.CreateCommand();
        dbCommandSelect.CommandText = $"SELECT itemID, quantity FROM INVENTORY_SLOT WHERE inventoryID = {inventoryID}";
        IDataReader dataReader = dbCommandSelect.ExecuteReader();

        while (dataReader.Read())
        {
            string itemID = dataReader.GetString(0); // APUNTE: Es el index de los valores del select, no de la tupla completa
            int quantity = dataReader.GetInt32(1);

            Item item = Resources.Load<Item>($"Items/{itemID}");

            if (item != null)
            {
                inventory.AddItem(item, quantity);
                //Debug.Log($"Se ha agregado un Item con ID: {itemID} y MaxStack: {item.GetMaxStack()}");
            }
            else
            {
                Debug.Log($"Item con ID: {itemID}, no encontrado en Resources/Item");
            }
        }

        dataReader.Close();
        dbCommandSelect.Dispose();

        dbConnection.Close();
    }

    public static bool CheckIfUserExists(string username)
    {
        bool userExists = false;

        IDbConnection dbConnection = CreateAndOpenDatabase();

        IDbCommand dbCommandCheck = dbConnection.CreateCommand();
        dbCommandCheck.CommandText = $"SELECT userID FROM USERS WHERE username = '{username}'";
        IDataReader reader = dbCommandCheck.ExecuteReader();

        if (reader.Read())
        {
            userExists = true;
        }
        
        reader.Close();
        dbCommandCheck.Dispose();

        dbConnection.Close();

        return userExists;
    }

    public static bool CheckIfPasswordCorrect(string username, string password)
    {
        bool passwordCorrect = false;

        IDbConnection dbConnection = CreateAndOpenDatabase();

        IDbCommand dbCommandCheck = dbConnection.CreateCommand();
        dbCommandCheck.CommandText = $"SELECT password FROM USERS WHERE username = '{username}'";
        IDataReader reader = dbCommandCheck.ExecuteReader();

        if (reader.Read())
        {
            passwordCorrect = reader.GetString(0) == password;
        }

        return passwordCorrect;
    }
}
