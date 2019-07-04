using net.esper.client;

namespace net.esper.example.autoid
{
	public class RFIDTagsPerSensorStmt
	{
	    private EPStatement statement;
	
	    public RFIDTagsPerSensorStmt(EPAdministrator admin)
	    {
            string stmt = "select ID as sensorId, coalesce(sum(countTags), 0) as numTagsPerSensor " +
                          "from AutoIdRFIDExample.win:time(60 sec) " +
                          "where Observation[0].Command = 'READ_PALLET_TAGS_ONLY' " +
                          "group by ID";
	
	        statement = admin.CreateEQL(stmt);
	    }
	
	    public void AddListener(UpdateListener listener)
	    {
	        statement.AddListener(listener);
	    }
	}
}
