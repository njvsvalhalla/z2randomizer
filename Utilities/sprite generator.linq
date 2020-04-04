<Query Kind="Program" />

void Main()
{
	var template = File.ReadAllText(@$"{Path.GetDirectoryName(Util.CurrentQueryPath)}\spriteTemplate.txt");

	var romPath = @"";
	var bytes = File.ReadAllBytes(romPath);
	var spriteName = "";

	template = template
		.Replace("{{name}}", spriteName)
		.Replace("{{spriteArr}}", GetSpriteString(bytes))
		.Replace("{{oneUpArr}}", GetString(new int[32], bytes, 0x20a90))
		.Replace("{{headArr}}", GetString(new int[16], bytes, 0x21970))
		.Replace("{{raftArr}}", GetString(new int[64], bytes, 0x31450))
		.Replace("{{owArr}}", GetString(new int[128], bytes, 0x31750))
		.Replace("{{titleArr}}", GetString(new int[32], bytes, 0x20D10))
		.Replace("{{sleeperArr}}", GetString(new int[96], bytes, 0x21010))
		.Replace("{{endOneArr}}", GetString(new int[256], bytes, 0x2ed90))
		.Replace("{{endTwoArr}}", GetString(new int[224], bytes, 0x2f010))
		.Replace("{{endThreeArr}}", GetString(new int[64], bytes, 0x2d010));

	File.WriteAllText(@$"C:\Users\Neal\Desktop\Utilities\{spriteName}Sprite.cs", template);
}

public string GetString(int[] arr, byte[] byteArr, int baseRef)
{
	for (var i = 0; i < arr.Count(); i++)
	{
		arr[i] = byteArr[baseRef + i];
	}
	return string.Join(", ", arr);
}
public string GetSpriteString(byte[] byteArr)
{
	var sprite = new int[4096];

	//sprite
	for (var i = 0; i < sprite.Count() * 3 / 8; i++)
	{
		sprite[i] = byteArr[0x20010 + i];
	}

	for (int i = 0; i < 0x20; i++)
	{
		sprite[0x6c0 + i] = byteArr[0x206d0 + i];
	}
	return string.Join(", ", sprite);
}