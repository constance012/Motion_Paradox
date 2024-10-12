using System;

public interface IUpgradeApplicationReceiver
{
	void OnUpgradeApplied(Type type, UpgradeBase upgrade);
}