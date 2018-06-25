using System;

[Serializable]
public class User
	{
	public int userID;
	public string firstName;
	public string surName;
	public string company;
	public string jobTitle;
	public string workEmail;
	public string phoneNumber;
	public int avatarID;
	public bool presenting;

	public string toString()
	{
		return "UserID:" + userID + ",firstname:" + firstName + ",surname:" + surName + ",company:" + company + ",jobtitle:" + jobTitle + ",workemail:" + workEmail + ",phonenum:" + phoneNumber + ",avatarid:" + avatarID + ",presenting?" + presenting;
	}

	public override bool Equals(object obj)
	{
		if (obj == null || GetType () != obj.GetType ())
			return false;
		
		User user = obj as User;
		return userID == user.userID
		&& firstName == user.firstName
		&& surName == user.surName
		&& company == user.company
		&& jobTitle == user.jobTitle
		&& workEmail == user.workEmail
		&& phoneNumber == user.phoneNumber
		&& avatarID == user.avatarID
		&& presenting == user.presenting;
	}
	
	}

