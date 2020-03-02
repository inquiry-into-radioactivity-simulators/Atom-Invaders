var iterationTime = 0.00;
var atomCount = 0;
var oneInXChance = 0.00; // chance
var background : Transform;
var countsLine : LineRenderer;
var atomsLine : LineRenderer;
var atomsLeft : TextMesh;
var countsText : TextMesh;
var iterationsText : TextMesh;

var showAtomsLeft = false;
var activated = false;

private var itr = 1;
private var originalatomCount = 0;
private var firstIteration = 0;
private var cam : Transform;

function Start ()
{
	cam = gameObject.Find("CameraMover").transform;
	/*
	atomsLeft.transform.parent = cam;
	countsText.transform.parent = cam;
	iterationsText.transform.parent = cam;
	background.transform.parent = cam;
	*/
	
	while(true)
	{
		if(!activated) return;
		
		if(atomCount > 0)
		{
			var thisIteration = 0;
			var curDice = atomCount;
			var i = 0;
			while(i < curDice)
			{
				if(Random.Range(0.00, oneInXChance) <= 1) 
				{
					atomCount --;
					thisIteration ++;
				}
				i ++;
			}
	
			if(originalatomCount == 0) originalatomCount = atomCount;
			if(firstIteration == 0) firstIteration = thisIteration;
	
			var curCountHeight = 0.0f;
			if(showAtomsLeft) curCountHeight = Mathf.Lerp(0, 160, parseFloat(thisIteration) / originalatomCount);
			else curCountHeight = Mathf.Lerp(0, 160, parseFloat(thisIteration) / firstIteration);
	
			iterationsText.transform.position.x = itr;
			iterationsText.text = "Iterations: " + itr;
			
			countsText.transform.position.y = curCountHeight;
			countsText.text = "Counts: " + thisIteration;
						
			countsLine.SetVertexCount(itr);
			countsLine.SetPosition( itr - 1, Vector3(itr, curCountHeight, 0));
			
			var curAtomHeight = 0.0f;
			if(showAtomsLeft)
			{
				curAtomHeight = Mathf.Lerp(0, 160, parseFloat(atomCount) / originalatomCount);
				
				atomsLeft.text = "Atoms Left: " + atomCount;
				atomsLeft.transform.position.y = curAtomHeight;
			
				atomsLine.SetVertexCount(itr);
				atomsLine.SetPosition( itr - 1, Vector3(itr, curAtomHeight, 0));
			}
			else
			{
				if(atomsLeft) Destroy(atomsLeft.gameObject);
				if(atomsLine) Destroy(atomsLine.gameObject);
			}
			
			if(itr > 150)
			{
				cam.position.x = itr + 10;
				cam.position.y = itr + 10 - 160;
				Camera.main.orthographicSize = 110 + ((itr - 160) * 1.1);
				var lineWidth = Mathf.Lerp(1, Camera.main.orthographicSize / 110, 0.5);
				
				countsText.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 110);
				iterationsText.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 110);
				countsLine.SetWidth(lineWidth, lineWidth);
				
				if(showAtomsLeft)
				{
					atomsLeft.transform.localScale = Vector3.one * (Camera.main.orthographicSize / 110);
					atomsLine.SetWidth(lineWidth, lineWidth);
				}
			}
			
			itr ++;
		}
		
		yield WaitForSeconds (iterationTime);
	}
}

