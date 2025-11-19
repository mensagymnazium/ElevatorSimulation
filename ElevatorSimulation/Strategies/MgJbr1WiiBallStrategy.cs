namespace ElevatorSimulation.Strategies;

/// <summary>
/// .........       kNNNNXoNNX.NNN' dXNNNXc lNNO NNN,   XNN'XNNNNNN.                
/// .........       OMMWNNxMMW.MMM,cMMWxMMW.dMMKlMMX    WMM,XNMMMWN.                
/// .........       OMMO  xMMW.MMM'xMMX WMM'dMMKKMM;    WMM,  MMMc                  
/// ..........      OMMKo:xMMW.MMM'kMMX WMM,dMMWMMK     WMM,  MMMc                  
/// ..........      OMMMMOxMMM.MMM'kMMX ... dMMMMMO     WMM,  MMMc                  
/// ..........      OMMKdcxMMM.MMM'kMMX odd.dMMWMMN     WMM,  MMMc                  
/// ...........     OMMO  xMMM.MMM,kMMX MMM'dMMKXMMl    WMM,  MMMc                  
/// ...........     OMMO..lMMM'MMM.oMMN.MMM.dMMKxMMN    WMM,  MMMc                  
/// ................OMMO   KMMMMMO  XMMMMMk dMMK:MMM:   WMM,  MMMc                  
/// ...'''''.....   :oo:    :kOkc    lkOx;  ,kOxxodo;   ood.  ooo.                  
/// ......                        ,:.        lll:                                   
///                              lXNK.       c::; .l:                               
///                              :clol       ;c:;:WWWx                              
///      dK0o           .....    .cccl.      ',;'dkOOx                              
///     oNWNXl    ......          'ccc'      '..'llccl                              
///     cxxxxo....           .lo.  .;;,k:   ;kOkxcc:c:                              
/// ...':ccccc              'NWWX; .','x0o .O0KK0lc:c'  'k0d                        
/// ..:clc:::c,             lO0Oko .::;cok;cOkOOkcc:c. .KNWNO                       
///  .:clc:::lxd.           lccccc .:::;:cccddxxo,..,  :ddddd                       
///  .xOdl::ccxOk'          ;cccc;  ;::::cc:lllcd,:';. ,lcccc                       
/// cWWMMK,.,,dOdc.         ,cccc'  .;:::::;cc:dX0OOxo''lccc:                       
/// kNWNKKo;',xdc;OOd:.     .lccc    .;:::;'cclKK0K0OO,,clcc'                       
/// dxkkxxxOOdK0.cO0XNXk.  ,c,,:,:     .::::ccx0kkOkxkcd;:::c..                     
/// :llllll0K00Kx.:okKXKl '0l,'.,k;..,:clcccccxxxdddodkx'''.dx,,'..                 
/// ,::::cckKOO0O, .;clc, dkdc:;:od:cclooooclloollllcoxo::::ok,,,,''..              
///  ::::ccxOkxkkl   ...  ddl:::clo:ccodooocooccc::::oolc:::coc;;,,,,,,'..          
///  ,c::ccodooddl    .',;olc:::cclccoodddoooolcc:::cclcc:::ccc:::;;;,,,,,'...      
///   :;:c::cccccc.';:::cccccccccclclddddddoddolc:cccodxxxxxdolc:cccc:;;,,,,,,''..  
///   '..;',:::cccc:ccccccclccccclccodddkkxxdddxdocokOkkkkxkkO0Odlc::cc::;;,,,,,,,'.
///   ''..'oOcccccccccccccclccccllclodkOxxxxxddddxxOkOOxdddxKX000kx0oc:ccc::;;,,,,,,
///  .dxkl;0X0cccllcccccccccclccllllokkxkdodxkxdxdxxkkxdolldK0O000kkdddoc:ccc::;;,,,
/// ;OKKXNWWWNkcllolllollllcclcllllldxkddxxdxOK0kkdxxdxkkkxxddxkO00xddddddlc:ccc::;;
/// c00NMMMMWWKololodddddlclllllllllxxxkddxO0XNWN0kdxoxkkkkkdddxkO00dddddddxdoc:::::
/// o0NMMMWNK0xkodllooodolcllooodllldxdoxdxOXWNNNNOddoo0kkkkxdddxO00xdddddddddddoc::
/// oNWWMWKkxodxodollloolllloxxxdlcloddlldxxKWWNWNkollc0OOkkxdddxO00xddddddddddddddo
/// 0WNNKOdolldooddllllllllldxkxlllclollccoxk0NNXOxc:c:kkkkXKdddxO00kddddddddddddddd
/// NXKOxlccloollddollllloloxxkkkdxd:cccc::codxxxoc:;;ckkkkKOdddxOOKxddddddddddddddd
/// X0kdlcccccccldddllllloooxxkkkkkxc::::;;;;:ccc:,;,,odddxkxddxkO0Odddddddddddddddd
/// 0xocccc::cccldddolllooodkkkkkkkxo;:::;,,,,,,,''',colllldxdxkO00ddOdddddddddddddd
/// xocccc:::cccodddooooooodkkkkkkxxdo:;;;;,,,''',,;:xxooooxxxkO00kdd0xddddddddddddd
/// ;cccc::cccccodxdooodooooxxkkkkxxoolc;;;;;;,,;;:okOOkkOOOOOO00kdddddddddddddddddd
/// .;:ccccccccldddoooooddooxxkkkkkxooddoc::;::::loxkOOOOO00O00kxdddOkdddddddddddddx
/// ''lllccccccdxddoodooodoodxxxxxxxoddddddoooooodooolxkOOOkkxddddddxdddddddddddxddd
/// ,loolocccoxkxddoooddoooooddxxxxxdxxddodddddddddddoodddodddooddddddddddddddddxxxx
/// cloodxxxxkxkkdxddooddoooxxoxxdodxxdddoddxxdoxddlddddododxodldxdooddddddddddddxxx
/// cooodxxxkxkkkoWMMo0MMX:NMM:MMN:XMMdddoXMMMMWkxdKMMMMdxKMMOdoMMNodddddddddddddddd
/// looodkkxkxxxkxKMMoKMMM'MMM;MMN,NMMdlooNMMkNMWclWMMMMxxKMMkooMMNoddddodddddddddxx
/// ldoodkkxxxxxxxkMMoNMMMcMMN:MMN,NMMdldoXMMd0MMcoMM0WMxxKMMkdoMMNlddddddddddxxdddx
/// oddddxxxxxxxxxxMMdMMMMkMMKoMMW:XMMddxoXMMNMWxodMMkNM0dKMMOdoMMNldddddddddddxxxdx
/// oodddxxxxxxxxxdMMOMMXMKMM0oMMWlXMMxxdoXMMKWMNcxMMdKMWlKMMOooMMNldddddddddddddxxx
/// oooddxdxxxxxxxdMMWMN0MNMMOlMMWcXMMxdxoNMMd0MMxOMM0NMMlKMMOdoMMNldoddddddddddddxx
/// oooooxdddxxdxxlMMMMOOMMMMolMMWcXMMdddoXMMoKMMdXMMWMMMoKMM0dcMMWooldodddddddddddd
/// doooodxddxxdxxcNMMMxlMMMM:lMMWcXMMdoolXMMMMMM;MMMo0MMxKMMMMlMMMMW:oddddddddddddd
/// —MgJbr
/// </summary>
public class MgJbr1WiiBallStrategy : IElevatorStrategy
{
	public MoveResult DecideNextMove(ElevatorSystem elevator)
	{
		if (SomeoneWantsToGetOn(elevator) || SomeoneWantsToGetOff(elevator))
		{
			return MoveResult.OpenDoors;
		}

		Direction direction = elevator.CurrentElevatorDirection;
		if (direction == Direction.Idle) direction = Direction.Up;
		for (int i = 0; i < 2; i++)
		{
			if (HasWorkInDirection(elevator, direction))
			{
				return MoveInDirection(direction);
			}
			direction = FlipDirection(direction);
		}

		return MoveResult.NoAction;
	}

	private static bool SomeoneWantsToGetOff(ElevatorSystem elevator)
	{
		return elevator.ActiveRiders.Exists((r) => r.To == elevator.CurrentElevatorFloor);
	}

	private static bool SomeoneWantsToGetOn(ElevatorSystem elevator)
	{
		return elevator.PendingRequests.Exists((r) => r.From == elevator.CurrentElevatorFloor && elevator.CurrentElevatorDirection switch
		{
			Direction.Up => r.To > elevator.CurrentElevatorFloor || elevator.CurrentElevatorFloor == elevator.Building.MaxFloor,
			Direction.Down => r.To < elevator.CurrentElevatorFloor || elevator.CurrentElevatorFloor == elevator.Building.MinFloor,
			_ => true,
		});
	}

	private static bool HasWorkInDirection(ElevatorSystem elevator, Direction direction)
	{
		return direction switch
		{
			Direction.Up => elevator.ActiveRiders.Exists((r) => r.To > elevator.CurrentElevatorFloor) || elevator.PendingRequests.Exists((r) => r.From > elevator.CurrentElevatorFloor),
			Direction.Down => elevator.ActiveRiders.Exists((r) => r.To < elevator.CurrentElevatorFloor) || elevator.PendingRequests.Exists((r) => r.From < elevator.CurrentElevatorFloor),
			_ => false,
		};
	}

	private static Direction FlipDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Up => Direction.Down,
			Direction.Down => Direction.Up,
			_ => throw new ArgumentException("Congratulations! You blew the elevator up and everyone died."),
		};
	}

	private static MoveResult MoveInDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Up => MoveResult.MoveUp,
			Direction.Down => MoveResult.MoveDown,
			_ => MoveResult.NoAction,
		};
	}
}
