using System;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells.Seventh;

namespace Server.Spells.Ninjitsu
{
	public class AnimalForm : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Animal Form", null, SpellCircle.Seventh, -1, 9002 );

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( EventSink_OnLogin );
		}

		private static void EventSink_OnLogin( LoginEventArgs e )
		{
			if ( m_Table.Contains( e.Mobile ) )
			{
				int body = e.Mobile.BodyMod;

				if ( body == 0x84 || body == 0x7A || body == 0xF6 || body == 0x19 || body == 0xDC || body == 0xDA )
				{
					e.Mobile.Send( new FastMovePacket( true ) );
				}
			}
		}

		public override double RequiredSkill { get { return 0.0; } }
		public override int RequiredMana { get { return 0; } }

		public override int CastDelayBase { get { return 0; } }
		public override int CastDelayCircleScalar { get { return 1; } }
		public override int CastDelayFastScalar { get { return 1; } }
		public override int CastDelayPerSecond { get { return 4; } }
		public override int CastDelayMinimum { get { return 2; } }

		public override int CastRecoveryBase { get { return 7; } }
		public override int CastRecoveryCircleScalar { get { return 0; } }
		public override int CastRecoveryFastScalar { get { return 1; } }
		public override int CastRecoveryPerSecond { get { return 4; } }
		public override int CastRecoveryMinimum { get { return 0; } }

		public override int SpellNumber { get { return -1; } }

		public override bool RevealOnCast { get { return false; } }

		public static Hashtable m_Table = new Hashtable();

		public static Hashtable m_Table2 = new Hashtable();

		public static Hashtable m_Table3 = new Hashtable();

		private void Expire_Callback( object state )
		{
			Mobile m = (Mobile) state;

			m.CloseGump( typeof( AnimalFormGump ) );

			m_Table3.Remove( m );

			FinishSequence();
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static int GetLastBody( Mobile m )
		{
			try
			{
				return (int) m_Table2[ m ];
			} 
			catch
			{
				return -1;
			}
		}

		private static int[] m_Bodys = new int[]
			{
				0x84,
				0x7A,
				0xF6,
				0x19,
				0xDC,
				0xDA,
				0x51,
				0x15,
				0xD9,
				0xC9,
				0xEE,					
				0xCD
			};

		public static object[] mods = new object[1]
					{
						new DefaultSkillMod( SkillName.Stealth, true, 20.0 )
					};

		public static bool CheckBody( int body )
		{
			bool flag = false;

			for ( int i = 0; i < m_Bodys.Length; i++ )
			{
				if ( body == m_Bodys[ i ] )
				{
					flag = true;
				}
			}

			return flag;
		}

		public static bool CheckMorph( Mobile m, int body )
		{
			double skill = m.Skills[ SkillName.Ninjitsu ].Value;

			double chance = skill/50.0;

			skill /= 100.0;

			if ( body == 0xEE || body == 0xCD )
			{
				chance = 1.3*skill + 7.0/120.0;
			}

			if ( body == 0xD9 || body == 0xC9 )
			{
				chance = 1.4*skill - 71.0/300.0;
			}

			if ( body == 0x84 || body == 0x7A )
			{
				chance = 1.85*skill - 979.0/600.0;
			}

			if ( chance < 0 )
			{
				chance = 0;
			}

			if ( chance > 1 )
			{
				chance = 1;
			}

			m.CheckSkill( SkillName.Ninjitsu, chance ); // passive gain Ninjitsu

			return (chance >= Utility.RandomDouble());
		}

		public static void Morph( Mobile from, int body )
		{
			if ( CheckMorph( from, body ) )
			{
				IMount mount = from.Mount;

				if ( mount != null )
				{
					mount.Rider = null;
				}

				from.BodyMod = body;

				if ( body == 0xEE || body == 0xCD )
				{
					from.AddSkillMod( (SkillMod) mods[ 0 ] );
				}

				if ( body == 0x51 )
				{
					from.HueMod = 0x5A3;
				}

				if ( body == 0x19 || body == 0xF6 )
				{
					from.Hits += 20;
				}

				if ( body == 0x84 || body == 0x7A || body == 0xF6 || body == 0x19 || body == 0xDC || body == 0xDA )
				{
					from.Send( new FastMovePacket( true ) );
				}

				m_Table[ from ] = true;

				m_Table2[ from ] = body;
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

				from.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );

				from.PlaySound( 0x1D6 );

				from.NextSpellTime = DateTime.Now;
			}
		}

		public AnimalForm( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( !Caster.CanBeginAction( typeof( PolymorphSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
				return false;
			}

			if ( Necromancy.TransformationSpell.UnderTransformation( Caster ) )
			{
				Caster.SendLocalizedMessage( 1063219 ); // You cannot mimic an animal while in that form.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckBody( Caster.BodyMod ) )
			{
				Caster.BodyMod = 0;

				Caster.HueMod = -1;

				Caster.RemoveSkillMod( (SkillMod) mods[ 0 ] );

				Caster.Send( new FastMovePacket( false ) );

				m_Table.Remove( Caster );
			}
			else
			{
				bool running = ((Caster.Direction & Direction.Running) != 0);

				int lastbody = GetLastBody( Caster );

				if ( running && CheckBody( lastbody ) )
				{
					Morph( Caster, lastbody );
				}
				else
				{
					Caster.CloseGump( typeof( AnimalFormGump ) );

					Caster.SendGump( new AnimalFormGump( Caster ) );

					Timer t = (Timer) m_Table3[ Caster ];

					if ( t != null )
					{
						t.Stop();
					}

					m_Table3[ Caster ] = t = Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerStateCallback( Expire_Callback ), Caster );
				}
			}

			FinishSequence();
		}
	}

	public class AnimalFormGump : Gump
	{
		private Mobile from;

		public AnimalFormGump( Mobile m ) : base( 50, 50 )
		{
			from = m;

			double value = from.Skills[ SkillName.Ninjitsu ].Value;

			AddPage( 0 );

			AddBackground( 0, 0, 408, 298, 0x13BE );
			AddBackground( 4, 28, 400, 240, 0xBB8 );

			AddHtmlLocalized( 4, 4, 400, 20, 1063394, 0x0, false, false ); // <center>Animal Form Selection Menu</center>

			AddButton( 25, 272, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 60, 274, 150, 20, 1011036, 0x0, false, false ); // OKAY

			AddButton( 285, 272, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 320, 274, 150, 20, 1011012, 0x0, false, false ); // CANCEL

			AddHtmlLocalized( 10, 30, 100, 18, 1029632, 0x0, false, false ); // kirin 
			AddHtmlLocalized( 110, 30, 100, 18, 1018214, 0x0, false, false ); // unicorn
			AddHtmlLocalized( 210, 30, 100, 18, 1030083, 0x0, false, false ); // bake-kitsune
			AddHtmlLocalized( 310, 30, 100, 18, 1028482, 0x0, false, false ); // wolf
			AddHtmlLocalized( 10, 110, 100, 18, 1028438, 0x0, false, false ); // llama
			AddHtmlLocalized( 110, 110, 100, 18, 1018273, 0x0, false, false ); // ostard
			AddHtmlLocalized( 210, 110, 100, 18, 1028496, 0x0, false, false ); // bullfrog
			AddHtmlLocalized( 310, 110, 100, 18, 1018114, 0x0, false, false ); // giant serpent
			AddHtmlLocalized( 10, 190, 100, 18, 1018280, 0x0, false, false ); // dog
			AddHtmlLocalized( 110, 190, 100, 18, 1018264, 0x0, false, false ); // cat
			AddHtmlLocalized( 210, 190, 100, 18, 1018294, 0x0, false, false ); // rat
			AddHtmlLocalized( 310, 190, 100, 18, 1028485, 0x0, false, false ); // rabbit

			if ( value >= 100.0 )
			{
				AddRadio( 10, 50, 0xD2, 0xD3, false, 101 ); // kirin
				AddItem( 40, 50, 9632 );
				AddToolTip( 1070811 ); // Increases movement speed and regenerates stamina quickly.

				AddRadio( 110, 50, 0xD2, 0xD3, false, 102 ); // unikorn
				AddItem( 140, 50, 9678 );
				AddToolTip( 1070812 ); // Increases movement speed and grants immunity to low level poisons.
			}
			else
			{
				AddItem( 10, 50, 9632, 0x3E3 ); // kirin
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.

				AddItem( 110, 50, 9678, 0x3E3 ); // unicorn
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.
			}

			if ( value >= 85.0 )
			{
				AddRadio( 210, 50, 0xD2, 0xD3, false, 103 ); // bake kitsune
				AddItem( 240, 50, 10083 );
				AddToolTip( 1070810 ); // Increases movement speed and grants a bonus to both your hit chance and maximum hit points.

				AddRadio( 310, 50, 0xD2, 0xD3, false, 104 ); // wolf
				AddItem( 340, 50, 9681, 0x905 );
				AddToolTip( 1070810 ); // Increases movement speed and grants a bonus to both your hit chance and maximum hit points.
			}
			else
			{
				AddItem( 210, 50, 10083, 0x3E3 ); // bake kitsune
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.

				AddItem( 310, 50, 9681, 0x3E3 ); // wolf
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.
			}

			if ( value >= 70.0 )
			{
				AddRadio( 10, 130, 0xD2, 0xD3, false, 105 ); // llama
				AddItem( 40, 130, 8438, 0 );
				AddToolTip( 1070809 ); // Increases movement speed.

				AddRadio( 110, 130, 0xD2, 0xD3, false, 106 ); // ostard
				AddItem( 140, 130, 8503, 0x8A4 );
				AddToolTip( 1070809 ); // Increases movement speed.				
			}
			else
			{
				AddItem( 10, 130, 8438, 0x3E3 ); // llama
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.

				AddItem( 110, 130, 8503, 0x3E3 ); // ostard
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.
			}

			if ( value >= 50.0 )
			{
				AddRadio( 210, 130, 0xD2, 0xD3, false, 107 ); // bullfrog
				AddItem( 240, 130, 8496, 0x7D3 );
				AddToolTip( 1070807 ); // Inflicts poison when your enemy damages you at short range.

				AddRadio( 310, 130, 0xD2, 0xD3, false, 108 ); // giant serpent
				AddItem( 340, 130, 9663, 0x7D9 );
				AddToolTip( 1070808 ); // Inflicts low level poison whenever you strike your opponent with a non-ranged weapon.
			}
			else
			{
				AddItem( 210, 130, 8496, 0x3E3 ); // bull frog
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.

				AddItem( 310, 130, 9663, 0x3E3 ); // giant serpent
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.
			}

			if ( value >= 40.0 )
			{
				AddRadio( 10, 210, 0xD2, 0xD3, false, 109 ); // dog
				AddItem( 40, 210, 8476, 0x905 );
				AddToolTip( 1070806 ); // Increases regeneration rate.  The increase is based on your Ninjitsu skill.

				AddRadio( 110, 210, 0xD2, 0xD3, false, 110 ); // cat
				AddItem( 140, 210, 8475, 0x905 );
				AddToolTip( 1070806 ); // Increases regeneration rate.  The increase is based on your Ninjitsu skill.
			}
			else
			{
				AddItem( 10, 210, 8476, 0x3E3 ); // dog
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.

				AddItem( 110, 210, 8475, 0x3E3 ); // cat
				AddToolTip( 1070708 ); // You do not have sufficient knowledge of Ninjitsu to assume this form.
			}

			AddRadio( 210, 210, 0xD2, 0xD3, false, 111 ); // rat
			AddItem( 240, 210, 8483, 0x905 );
			AddToolTip( 1070805 ); // Grants a bonus to the Stealth skill.

			AddRadio( 310, 210, 0xD2, 0xD3, false, 112 ); // rabbit
			AddItem( 340, 210, 8485, 0x905 );
			AddToolTip( 1070805 ); // Grants a bonus to the Stealth skill.
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 && info.Switches.Length > 0 )
			{
				int cnum = info.Switches[ 0 ];

				switch ( cnum )
				{
					case 101:
						{
							AnimalForm.Morph( from, 0x84 ); // kirin
							break;
						}
					case 102:
						{
							AnimalForm.Morph( from, 0x7A ); // unikorn
							break;
						}
					case 103:
						{
							AnimalForm.Morph( from, 0xF6 ); // bake kitsune
							break;
						}
					case 104:
						{
							AnimalForm.Morph( from, 0x19 ); // wolf
							break;
						}
					case 105:
						{
							AnimalForm.Morph( from, 0xDC ); // llama
							break;
						}
					case 106:
						{
							AnimalForm.Morph( from, 0xDA ); // ostard
							break;
						}
					case 107:
						{
							AnimalForm.Morph( from, 0x51 ); // bullfrog
							break;
						}
					case 108:
						{
							AnimalForm.Morph( from, 0x15 ); // giant serpent
							break;
						}
					case 109: // dog
						{
							AnimalForm.Morph( from, 0xD9 );
							break;
						}
					case 110:
						{
							AnimalForm.Morph( from, 0xC9 ); // cat
							break;
						}
					case 111:
						{
							AnimalForm.Morph( from, 0xEE ); // rat
							break;
						}
					case 112:
						{
							AnimalForm.Morph( from, 0xCD ); // rabbit
							break;
						}
				}
			}
		}
	}
}