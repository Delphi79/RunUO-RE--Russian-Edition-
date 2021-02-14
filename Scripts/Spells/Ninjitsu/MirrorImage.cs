using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Reflection;

namespace Server.Spells.Ninjitsu
{
	public class MirrorImage : NinjaSpell
	{
		private static SpellInfo m_Info = new SpellInfo( "Mirror Image", null, SpellCircle.Seventh, -1, 0 );

		public override double RequiredSkill { get { return 40.0; } }
		public override int RequiredMana { get { return 10; } }

		public override int CastDelayBase { get { return 0; } }
		public override int CastDelayCircleScalar { get { return 1; } }
		public override int CastDelayFastScalar { get { return 2; } }
		public override int CastDelayPerSecond { get { return 4; } }
		public override int CastDelayMinimum { get { return 1; } }

		public override int CastRecoveryBase { get { return 6; } }
		public override int CastRecoveryCircleScalar { get { return 0; } }
		public override int CastRecoveryFastScalar { get { return 2; } }
		public override int CastRecoveryPerSecond { get { return 4; } }
		public override int CastRecoveryMinimum { get { return 0; } }

		public override int SpellNumber { get { return -1; } }

		public MirrorImage( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
			{
				return false;
			}

			if ( Caster.Mounted )
			{
				Caster.SendLocalizedMessage( 1010097 ); // You cannot use this while mounted.
				return false;
			}

			if ( (Caster.Followers + 1) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1063133 ); // You cannot summon a mirror image because you have too many followers.
				return false;
			}

			Caster.SendLocalizedMessage( 1063134 ); // You begin to summon a mirror image of yourself.			

			return true;
		}

		public static bool ComputeChance( Mobile m )
		{
			double skill = m.Skills[ SkillName.Ninjitsu ].Value;

			skill /= 100.0;

			double chance = 2.5*skill - 449.0/600.0;

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

		public override void OnCast()
		{
			if ( ComputeChance( Caster ) )
			{
				Caster.FixedParticles( 0x376A, 1, 14, 0x13B5, 0, 0, EffectLayer.Waist );

				Clone clone = new Clone( Caster );

				clone.MoveToWorld( Caster.Location, Caster.Map );

				Caster.Mana -= RequiredMana;
			}
			else
			{
				Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

				Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );

				Caster.PlaySound( 0x1D6 );

				Caster.NextSpellTime = DateTime.Now;
			}

			FinishSequence();
		}
	}

	public class Clone : BaseCreature
	{
		public Mobile m_Owner;

		public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

		public Clone( Mobile caster ) : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			m_Owner = caster;
			Copy();
		}

		public Clone( Serial serial ) : base( serial )
		{
		}

		public void CopyProperties( Item dest, Item src )
		{
			PropertyInfo[] props = src.GetType().GetProperties();

			for ( int i = 0; i < props.Length; i++ )
			{
				try
				{
					if ( props[ i ].CanRead && props[ i ].CanWrite )
					{
						props[ i ].SetValue( dest, props[ i ].GetValue( src, null ), null );
					}
				} 
				catch
				{
				}
			}
		}

		public void Copy()
		{
			if ( m_Owner != null )
			{
				m_Owner.Followers += 1;

				BodyMod = m_Owner.Body;

				Name = m_Owner.Name;

				Title = m_Owner.Title;

				Fame = m_Owner.Fame;

				Female = m_Owner.Female;

				NameHue = m_Owner.NameHue;

				SpeechHue = m_Owner.SpeechHue;

				Criminal = m_Owner.Criminal;

				Karma = m_Owner.Karma;

				Kills = m_Owner.Kills;

				Hue = m_Owner.Hue;

				Str = m_Owner.Str;

				Dex = m_Owner.Dex;

				Int = m_Owner.Int;

				RawStr = m_Owner.RawStr;

				RawDex = m_Owner.RawDex;

				RawInt = m_Owner.RawInt;

				StatCap = m_Owner.StatCap;

				SkillsCap = m_Owner.SkillsCap;

				Hits = m_Owner.Hits;

				Mana = m_Owner.Mana;

				Stam = m_Owner.Stam;

				for ( int i = 0; i < m_Owner.Skills.Length; ++i )
				{
					Skills[ i ].Base = m_Owner.Skills[ i ].Base;
					Skills[ i ].Cap = m_Owner.Skills[ i ].Cap;
				}

				ArrayList items = new ArrayList( m_Owner.Items );
				for ( int i = 0; i < items.Count; i++ )
				{
					Item item = (Item) items[ i ];
					if ( ((item != null) && (item.Parent == m_Owner) && (item != m_Owner.Backpack)) )
					{
						Type type = item.GetType();
						Item newitem = Loot.Construct( type );
						CopyProperties( newitem, item );
						AddItem( newitem );
					}
				}
				AddItem( new Backpack() );
			}
			Timer.DelayCall( TimeSpan.FromSeconds( Utility.Random( 30, 60 ) ), new TimerCallback( remove_Clon ) );
		}

		public void remove_Clon()
		{
			try
			{
				Effects.SendLocationParticles( EffectItem.Create( this.Location, this.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );

				Delete();
			} 
			catch
			{
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Owner = reader.ReadMobile() as Mobile;

			Timer.DelayCall( TimeSpan.FromSeconds( Utility.Random( 30 ) ), new TimerCallback( remove_Clon ) );
		}

		public override bool OnBeforeDeath()
		{
			remove_Clon();

			return false;
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			remove_Clon();
		}

		public override void OnDamagedBySpell( Mobile from )
		{
			remove_Clon();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			try
			{
				if ( this.Owner != null )
				{
					(this.Owner).Followers -= 1;
				}

				if ( this.Owner != null && (this.Owner).Followers < 0 )
				{
					(this.Owner).Followers = 0;
				}
			} 
			catch
			{
			}
		}
	}
}