using System;
using System.Windows.Forms;
using ScriptPortal.Vegas;
//If you are using Vegas version under 13, then change ScriptPortal.Vegas to Sony.Vegas
public class EntryPoint
{  
    public void FromVegas(Vegas myVegas)
    {
        foreach (Track myTrack in myVegas.Project.Tracks)
        {
            if (myTrack.Selected)
            {
				if (myTrack.IsVideo())
				{
					int objectNum = 0;
					//int nthObject = 4; //If you want to change the Nth object to select, change this number to N which you want.
					TrackEvent prevEvent = null;
					foreach(TrackEvent myEvent in myTrack.Events)
					{					
						if (myEvent.Start >= myVegas.SelectionStart && myEvent.Start <= (myVegas.SelectionStart + myVegas.SelectionLength))
						{
		
							//myEvent.Selected = true;
							
							VideoEvent vevnt = (VideoEvent)myEvent;
							var VelEnv = new Envelope(EnvelopeType.Velocity);		
							if (vevnt.Envelopes.Count == 0) 
							{
														
								vevnt.Envelopes.Add(VelEnv);
							}
							var NanosEndFrame = myEvent.End.Nanos;
							var NanosEndFrameStr = NanosEndFrame.ToString();
							var EndFrame = myEvent.End.FrameCount;
							var EndFrameStr = EndFrame.ToString();
							var NanosOfFrame = Timecode.FromFrames(EndFrame).Nanos;
							var NanosOfFrameStr = NanosOfFrame.ToString();
							//MessageBox.Show("actual nano end: " + NanosEndFrameStr + "frames nano end: " + NanosOfFrameStr);
							int thePoint = 0;
							if (NanosEndFrame == NanosOfFrame) 
							{
								//MessageBox.Show("frame = nano");
								thePoint = Convert.ToInt32(EndFrame) - 1;
							} else if (NanosOfFrame < NanosEndFrame){
								//MessageBox.Show("frame < nano");
								thePoint = Convert.ToInt32(EndFrame);
							} else {
								//MessageBox.Show("frame is after the nano end frame");
							}
							//MessageBox.Show("last frame is: " + thePoint.ToString());
							var EndFrameTimecode = Timecode.FromFrames(Convert.ToInt64(thePoint));
							double zero = 0;
							foreach (Envelope env in vevnt.Envelopes)
							{
								if (env.Type == EnvelopeType.Velocity)
								{
									bool PointExists = false;
									var LengthPoint = EndFrameTimecode - myEvent.Start;
									foreach (EnvelopePoint envPoint in env.Points)
									{
										if (envPoint.Index == 0)
										{
											envPoint.Curve = CurveType.None;
										}
										//MessageBox.Show(envPoint.X.ToString(RulerFormat.AbsoluteFrames) + " " +LengthPoint.ToString(RulerFormat.AbsoluteFrames));
										if (envPoint.X == LengthPoint)
										{
											PointExists = true;
										}
									}
									if (!PointExists)
									{
										env.Points.Add(new EnvelopePoint(LengthPoint, zero, CurveType.None	));
									}
								}
							}
							if (prevEvent != null)
							{
								prevEvent.Length = myEvent.Start  - prevEvent.Start;
							}
							prevEvent = myEvent;
						}
					
					}
				}
            }
        }
    }
}

