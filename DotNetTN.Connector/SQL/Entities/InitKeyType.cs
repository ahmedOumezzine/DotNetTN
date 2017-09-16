namespace DotNetTN.Connector.SQL.Entities
{
    public enum InitKeyType
    {
        /// <summary>
        /// Init primary key and identity key from the system table
        /// </summary>
        SystemTable = 0,

        /// <summary>
        /// Init primary key and identity key from the attribute
        /// </summary>
        Attribute = 1
    }
}