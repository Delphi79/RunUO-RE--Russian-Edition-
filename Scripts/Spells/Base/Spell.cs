using System;
using Server.Items;
using Server.Network;
using Server.Spells.Bushido;
using Server.Spells.Chivalry;
using Server.Spells.Ninjitsu;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells.Second;
using Server.Spells.Necromancy;

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private Mobile m_Caster;
		private Item m_Scroll;
		private SpellInfo m_Info;
		private SpellState m_State;
		private DateTime m_StartCastTime;

		public SpellState State { get { return m_State; } set { m_State = value; } }
		public Mobile Caster { get { return m_Caster; } }
		public SpellInfo Info { get { return m_Info; } }
		public string Name { get { return m_Info.Name; } }
		public string Mantra { get { return m_Info.Mantra; } }
		public SpellCircle Circle { get { return m_Info.Circle; } }
		public Type[] Reagents { get { return m_Info.Reagents; } }
		public Item Scroll { get { return m_Scroll; } }

		private static TimeSpan NextSpellDelay = TimeSpan.FromSeconds( 1.25 );
		private static TimeSpan AnimateDelay = TimeSpan.FromSeconds( 1.5 );

		public virtual SkillName CastSkill { get { return SkillName.Magery; } }
		public virtual SkillName DamageSkill { get { return SkillName.EvalInt; } }

		public virtual bool RevealOnCast { get { return true; } }
		public virtual bool ClearHandsOnCast { get { return true; } }

		public virtual bool DelayedDamage { get { return false; } }

		public Spell( Mobile caster, Item scroll, SpellInfo info )
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;
		}

		public virtual int GetNewAosDamage( int bonus, int dice, int sides, bool playerVsPlayer )
		{
			int damage = Utility.Dice( dice, sides, bonus )*100;
			int damageBonus = 0;

			int inscribeSkill = GetInscribeFixed( m_Caster );
			int inscribeBonus = (inscribeSkill + (1000*(inscribeSkill/1000)))/200;
			damageBonus += inscribeBonus;

			int intBonus = Caster.Int/10;
			damageBonus += intBonus;

			int sdiBonus = AosAttributes.GetValue( m_Caster, AosAttribute.SpellDamage );
			// PvP spell damage increase cap of 15% from an item�s magic property
			if ( playerVsPlayer && sdiBonus > 15 )
			{
				sdiBonus = 15;
			}
			damageBonus += sdiBonus;

			damage = AOS.Scale( damage, 100 + damageBonus );

			int evalSkill = GetDamageFixed( m_Caster );
			int evalScale = 30 + ((9*evalSkill)/100);

			damage = AOS.Scale( damage, evalScale );

			return damage/100;
		}

		public virtual double GetAosDamage( int min, int random, double div )
		{
			double scale = 1.0;

			scale += GetInscribeSkill( m_Caster )*0.001;

			if ( Caster.Player )
			{
				scale += Caster.Int*0.001;
				scale += AosAttributes.GetValue( m_Caster, AosAttribute.SpellDamage )*0.01;
			}

			int baseDamage = min + (int) (GetDamageSkill( m_Caster )/div);

			double damage = Utility.RandomMinMax( baseDamage, baseDamage + random );

			return damage*scale;
		}

		public virtual bool IsCasting { get { return m_State == SpellState.Casting; } }

		public virtual void OnCasterHurt()
		{
			if ( IsCasting )
			{
				object o = ProtectionSpell.Registry[ m_Caster ];
				bool disturb = true;

				if ( o != null && o is double )
				{
					if ( ((double) o) > Utility.RandomDouble()*100.0 )
					{
						disturb = false;
					}
				}

				if ( m_Caster is BaseCreature )
					disturb = false;

				if ( disturb )
				{
					Disturb( DisturbType.Hurt, false, true );
				}
			}
		}

		public virtual void OnCasterKilled()
		{
			Disturb( DisturbType.Kill );
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving( Direction d )
		{
			if ( IsCasting && BlocksMovement )
			{
				m_Caster.SendLocalizedMessage( 500111 ); // You are frozen and can not move.
				return false;
			}

			return true;
		}

		public virtual bool OnCasterEquiping( Item item )
		{
			if ( IsCasting )
			{
				Disturb( DisturbType.EquipRequest );
			}

			return true;
		}

		public virtual bool OnCasterUsingObject( object o )
		{
			if ( m_State == SpellState.Sequencing )
			{
				Disturb( DisturbType.UseRequest );
			}

			return true;
		}

		public virtual bool OnCastInTown( Region r )
		{
			return m_Info.AllowTown;
		}

		public virtual bool ConsumeReagents()
		{
			if ( m_Scroll != null || !m_Caster.Player )
			{
				return true;
			}

			if ( AosAttributes.GetValue( m_Caster, AosAttribute.LowerRegCost ) > Utility.Random( 100 ) )
			{
				return true;
			}

			Container pack = m_Caster.Backpack;

			if ( pack == null )
			{
				return false;
			}

			if ( pack.ConsumeTotal( m_Info.Reagents, m_Info.Amounts ) == -1 )
			{
				return true;
			}

			if ( GetType().BaseType == typeof( Spell ) )
			{
				if ( ArcaneGem.ConsumeCharges( m_Caster, 1 ) )
				{
					return true;
				}
			}

			return false;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if ( n <= 0.0 )
			{
				return false;
			}

			if ( n >= 1.0 )
			{
				return true;
			}

			int maxSkill = (1 + (int) Circle)*10;
			maxSkill += (1 + ((int) Circle/6))*25;

			if ( target.Skills[ SkillName.MagicResist ].Value < maxSkill )
			{
				target.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );
			}

			return (n >= Utility.RandomDouble());
		}

		public virtual double GetInscribeSkill( Mobile m )
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[ SkillName.Inscribe ].Value;
		}

		public virtual int GetInscribeFixed( Mobile m )
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[ SkillName.Inscribe ].Fixed;
		}

		public virtual int GetDamageFixed( Mobile m )
		{
			m.CheckSkill( DamageSkill, 0.0, 120.0 );

			return m.Skills[ DamageSkill ].Fixed;
		}

		public virtual double GetDamageSkill( Mobile m )
		{
			m.CheckSkill( DamageSkill, 0.0, 120.0 );

			return m.Skills[ DamageSkill ].Value;
		}

		public virtual int GetResistFixed( Mobile m )
		{
			int maxSkill = (1 + (int) Circle)*10;
			maxSkill += (1 + ((int) Circle/6))*25;

			if ( m.Skills[ SkillName.MagicResist ].Value < maxSkill )
			{
				m.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );
			}

			return m.Skills[ SkillName.MagicResist ].Fixed;
		}

		public virtual double GetResistSkill( Mobile m )
		{
			int maxSkill = (1 + (int) Circle)*10;
			maxSkill += (1 + ((int) Circle/6))*25;

			if ( m.Skills[ SkillName.MagicResist ].Value < maxSkill )
			{
				m.CheckSkill( SkillName.MagicResist, 0.0, 120.0 );
			}

			return m.Skills[ SkillName.MagicResist ].Value;
		}

		public virtual double GetResistPercentForCircle( Mobile target, SpellCircle circle )
		{
			double firstPercent = target.Skills[ SkillName.MagicResist ].Value/5.0;
			double secondPercent = target.Skills[ SkillName.MagicResist ].Value - (((m_Caster.Skills[ CastSkill ].Value - 20.0)/5.0) + (1 + (int) circle)*5.0);

			return (firstPercent > secondPercent ? firstPercent : secondPercent)/2.0; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target, m_Info.Circle );
		}

		public virtual double GetDamageScalar( Mobile target )
		{
			double casterEI = m_Caster.Skills[ DamageSkill ].Value;
			double targetRS = target.Skills[ SkillName.MagicResist ].Value;
			double scalar;

			if ( Core.AOS )
			{
				targetRS = 0;
			}

			m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

			if ( casterEI > targetRS )
			{
				scalar = (1.0 + ((casterEI - targetRS)/500.0));
			}
			else
			{
				scalar = (1.0 + ((casterEI - targetRS)/200.0));
			}

			// magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 120 skill
			scalar += (m_Caster.Skills[ CastSkill ].Value - 100.0)/400.0;

			if ( !target.Player && !target.Body.IsHuman && !Core.AOS )
			{
				scalar *= 2.0; // Double magery damage to monsters/animals if not AOS
			}

			if ( target is BaseCreature )
			{
				((BaseCreature) target).AlterDamageScalarFrom( m_Caster, ref scalar );
			}

			if ( m_Caster is BaseCreature )
			{
				((BaseCreature) m_Caster).AlterDamageScalarTo( target, ref scalar );
			}

			target.Region.SpellDamageScalar( m_Caster, target, ref scalar );

			return scalar;
		}

		public virtual void DoFizzle()
		{
			m_Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502632 ); // The spell fizzles.

			if ( m_Caster.Player )
			{
				if ( Core.AOS )
				{
					m_Caster.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				}
				else
				{
					m_Caster.FixedEffect( 0x3735, 6, 30 );
				}

				m_Caster.PlaySound( 0x5C );
			}
		}

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public void Disturb( DisturbType type )
		{
			Disturb( type, true, false );
		}

		public virtual bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if ( resistable && m_Scroll is BaseWand )
			{
				return false;
			}

			return true;
		}

		public void Disturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if ( !CheckDisturb( type, firstCircle, resistable ) )
			{
				return;
			}

			if ( m_State == SpellState.Casting )
			{
				if ( !firstCircle && Circle == SpellCircle.First && !Core.AOS )
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, true );

				if ( m_CastTimer != null )
				{
					m_CastTimer.Stop();
				}

				if ( m_AnimTimer != null )
				{
					m_AnimTimer.Stop();
				}

				if ( Core.AOS && m_Caster.Player && type == DisturbType.Hurt )
				{
					DoHurtFizzle();
				}

				m_Caster.NextSpellTime = DateTime.Now + GetDisturbRecovery();
			}
			else if ( m_State == SpellState.Sequencing )
			{
				if ( !firstCircle && Circle == SpellCircle.First && !Core.AOS )
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb( type, false );

				Targeting.Target.Cancel( m_Caster );

				if ( Core.AOS && m_Caster.Player && type == DisturbType.Hurt )
				{
					DoHurtFizzle();
				}
			}
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect( 0x3735, 6, 30 );
			m_Caster.PlaySound( 0x5C );
		}

		public virtual void OnDisturb( DisturbType type, bool message )
		{
			if ( message )
			{
				m_Caster.SendLocalizedMessage( 500641 ); // Your concentration is disturbed, thus ruining thy spell.
			}
		}

		public virtual bool CheckCast()
		{
			return true;
		}

		public virtual void SayMantra()
		{
			if ( m_Scroll is BaseWand )
			{
				return;
			}

			if ( m_Info.Mantra != null && m_Info.Mantra.Length > 0 && m_Caster.Player )
			{
				m_Caster.PublicOverheadMessage( MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, false );
			}
		}

		public virtual bool BlockedByHorrificBeast { get { return true; } }
		public virtual bool BlocksMovement { get { return true; } }

		public virtual bool CheckNextSpellTime { get { return !(m_Scroll is BaseWand); } }

		public bool Cast()
		{
			m_StartCastTime = DateTime.Now;

			if ( Core.AOS && m_Caster.Spell is Spell && ((Spell) m_Caster.Spell).State == SpellState.Sequencing )
			{
				((Spell) m_Caster.Spell).Disturb( DisturbType.NewCast );
			}

			if ( !m_Caster.CheckAlive() )
			{
				return false;
			}
			else if ( m_Caster.Spell != null && m_Caster.Spell.IsCasting )
			{
				m_Caster.SendLocalizedMessage( 502642 ); // You are already casting a spell.
			}
			else if ( BlockedByHorrificBeast && TransformationSpell.UnderTransformation( m_Caster, typeof( HorrificBeastSpell ) ) )
			{
				m_Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
			}
			else if ( !( this is Shadowjump || this is MirrorImage || this is AnimalForm ) && AnimalForm.UnderEffect( m_Caster ) )
			{
				// TODO: Revealing action and mantra should be before this ( at least for palading spell)
				if ( this is PaladinSpell )
				{
					m_Caster.SendLocalizedMessage( 1063218 ); // You cannot use that ability in this form.
				}
				else if ( this is SamuraiSpell )
				{
					if ( this is MomentumStrike || this is LightningStrike || this is HonorableExecution )
					{
						m_Caster.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.
					}
					else
					{
						m_Caster.SendLocalizedMessage( 1063218 ); // You cannot use that ability in this form.
					}
				}
				else if (this is NinjaSpell)
				{
					m_Caster.SendLocalizedMessage( 1063024 ); // You cannot perform this special move right now.
				}
				else
				{
					m_Caster.SendLocalizedMessage( 1061091 ); // You cannot cast that spell in this form.
				}
			}
			else if ( !(m_Scroll is BaseWand) && (m_Caster.Paralyzed || m_Caster.Frozen) )
			{
				m_Caster.SendLocalizedMessage( 502643 ); // You can not cast a spell while frozen.
			}
			else if ( CheckNextSpellTime && DateTime.Now < m_Caster.NextSpellTime )
			{
				m_Caster.SendLocalizedMessage( 502644 ); // You must wait for that spell to have an effect.
			}
			else if ( m_Caster.Mana >= ScaleMana( GetMana() ) )
			{
				if ( m_Caster.Spell == null && m_Caster.CheckSpellCast( this ) && CheckCast() && m_Caster.Region.OnBeginSpellCast( m_Caster, this ) )
				{
					m_State = SpellState.Casting;
					m_Caster.Spell = this;

					if ( RevealOnCast )
					{
						m_Caster.RevealingAction();
					}

					SayMantra();

					TimeSpan castDelay = this.GetCastDelay();

					if ( m_Caster.Body.IsHuman )
					{
						int count = (int) Math.Ceiling( castDelay.TotalSeconds/AnimateDelay.TotalSeconds );

						if ( count != 0 )
						{
							m_AnimTimer = new AnimTimer( this, count );
							m_AnimTimer.Start();
						}

						if ( m_Info.LeftHandEffect > 0 )
						{
							Caster.FixedParticles( 0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand );
						}

						if ( m_Info.RightHandEffect > 0 )
						{
							Caster.FixedParticles( 0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand );
						}
					}

					if ( ClearHandsOnCast )
					{
						m_Caster.ClearHands();
					}

					m_CastTimer = new CastTimer( this, castDelay );
					m_CastTimer.Start();

					OnBeginCast();

					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana
			}

			return false;
		}

		public abstract void OnCast();

		public virtual void OnBeginCast()
		{
		}

		private const double ChanceOffset = 20.0, ChanceLength = 100.0/7.0;

		public virtual void GetCastSkills( out double min, out double max )
		{
			int circle = (int) m_Info.Circle;

			if ( m_Scroll != null )
			{
				circle -= 2;
			}

			double avg = ChanceLength*circle;

			min = avg - ChanceOffset;
			max = avg + ChanceOffset;
		}

		public virtual bool CheckFizzle()
		{
			if ( m_Scroll is BaseWand )
			{
				return true;
			}

			double minSkill, maxSkill;

			GetCastSkills( out minSkill, out maxSkill );

			return Caster.CheckSkill( CastSkill, minSkill, maxSkill );
		}

		private static int[] m_ManaTable = new int[] {4, 6, 9, 11, 14, 20, 40, 50};

		public virtual int GetMana()
		{
			if ( m_Scroll is BaseWand )
			{
				return 0;
			}

			return m_ManaTable[ (int) Circle ];
		}

		public virtual int ScaleMana( int mana )
		{
			double scalar = 1.0;

			if ( !Necromancy.MindRotSpell.GetMindRotScalar( Caster, ref scalar ) )
			{
				scalar = 1.0;
			}

			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue( m_Caster, AosAttribute.LowerManaCost );
			if ( lmc > 40 )
			{
				lmc = 40;
			}

			scalar -= (double) lmc/100;

			return (int) (mana*scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
			if ( Core.AOS )
			{
				return TimeSpan.Zero;
			}

			double delay = 1.0 - Math.Sqrt( (DateTime.Now - m_StartCastTime).TotalSeconds/GetCastDelay().TotalSeconds );

			if ( delay < 0.2 )
			{
				delay = 0.2;
			}

			return TimeSpan.FromSeconds( delay );
		}

		public virtual int CastRecoveryBase { get { return 6; } }
		public virtual int CastRecoveryCircleScalar { get { return 0; } }
		public virtual int CastRecoveryFastScalar { get { return 1; } }
		public virtual int CastRecoveryPerSecond { get { return 4; } }
		public virtual int CastRecoveryMinimum { get { return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
			if ( !Core.AOS )
			{
				return NextSpellDelay;
			}

			int fcr = AosAttributes.GetValue( m_Caster, AosAttribute.CastRecovery );

			int circleDelay = CastRecoveryCircleScalar*(1 + (int) Circle); // Note: Circle is 0-based so we must offset
			int fcrDelay = -(CastRecoveryFastScalar*fcr);

			int delay = CastRecoveryBase + circleDelay + fcrDelay;

			if ( delay < CastRecoveryMinimum )
			{
				delay = CastRecoveryMinimum;
			}

			return TimeSpan.FromSeconds( (double) delay/CastRecoveryPerSecond );
		}

		public virtual int CastDelayBase { get { return 3; } }
		public virtual int CastDelayCircleScalar { get { return 1; } }
		public virtual int CastDelayFastScalar { get { return 1; } }
		public virtual int CastDelayPerSecond { get { return 4; } }
		public virtual int CastDelayMinimum { get { return 1; } }

		public virtual TimeSpan GetCastDelay()
		{
			if ( m_Scroll is BaseWand )
			{
				return TimeSpan.Zero;
			}

			if ( !Core.AOS )
			{
				return TimeSpan.FromSeconds( 0.5 + (0.25*(int) Circle) );
			}

			// Faster casting cap of 2 (if not using the protection spell) 
			// Faster casting cap of 0 (if using the protection spell) 
			// Paladin spells are subject to a faster casting cap of 4 
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2 
			int fcMax = 2;

			if ( CastSkill == SkillName.Chivalry && m_Caster.Skills[ SkillName.Magery ].Value < 70.0 )
			{
				fcMax = 4;
			}

			int fc = AosAttributes.GetValue( m_Caster, AosAttribute.CastSpeed );

			if ( fc > fcMax )
			{
				fc = fcMax;
			}

			if ( ProtectionSpell.Registry.Contains( m_Caster ) )
			{
				fc -= 2;
			}

			int circleDelay = CastDelayCircleScalar*(1 + (int) Circle); // Note: Circle is 0-based so we must offset
			int fcDelay = -(CastDelayFastScalar*fc);

			int delay = CastDelayBase + circleDelay + fcDelay;

			if ( delay < CastDelayMinimum )
			{
				delay = CastDelayMinimum;
			}

			return TimeSpan.FromSeconds( (double) delay/CastDelayPerSecond );
		}

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;

			if ( m_Caster.Spell == this )
			{
				m_Caster.Spell = null;
			}
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			int mana = ScaleMana( GetMana() );

			if ( m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing )
			{
				DoFizzle();
			}
			else if ( m_Scroll != null && !(m_Scroll is Runebook) && (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster || (m_Scroll is BaseWand && (((BaseWand) m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))) )
			{
				DoFizzle();
			}
			else if ( !ConsumeReagents() )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502630 ); // More reagents are needed for this spell.
			}
			else if ( m_Caster.Mana < mana )
			{
				m_Caster.LocalOverheadMessage( MessageType.Regular, 0x22, 502625 ); // Insufficient mana for this spell.
			}
			else if ( Core.AOS && (m_Caster.Frozen || m_Caster.Paralyzed) )
			{
				m_Caster.SendLocalizedMessage( 502646 ); // You cannot cast a spell while frozen.
				DoFizzle();
			}
			else if ( CheckFizzle() )
			{
				m_Caster.Mana -= mana;

				if ( m_Scroll is SpellScroll )
				{
					m_Scroll.Consume();
				}
				else if ( m_Scroll is BaseWand )
				{
					((BaseWand) m_Scroll).ConsumeCharge( m_Caster );
				}

				if ( m_Scroll is BaseWand )
				{
					bool m = m_Scroll.Movable;

					m_Scroll.Movable = false;

					if ( ClearHandsOnCast )
					{
						m_Caster.ClearHands();
					}

					m_Scroll.Movable = m;
				}
				else
				{
					if ( ClearHandsOnCast )
					{
						m_Caster.ClearHands();
					}
				}

				int karma = ComputeKarmaAward();

				if ( karma != 0 )
				{
					Misc.Titles.AwardKarma( Caster, karma, true );
				}

				if ( TransformationSpell.UnderTransformation( m_Caster, typeof( VampiricEmbraceSpell ) ) )
				{
					bool garlic = false;

					for ( int i = 0; !garlic && i < m_Info.Reagents.Length; ++i )
					{
						garlic = (m_Info.Reagents[ i ] == Reagent.Garlic);
					}

					if ( garlic )
					{
						m_Caster.SendLocalizedMessage( 1061651 ); // The garlic burns you!
						AOS.Damage( m_Caster, Utility.RandomMinMax( 17, 23 ), 100, 0, 0, 0, 0 );
					}
				}

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence( Mobile target )
		{
			return CheckBSequence( target, false );
		}

		public bool CheckBSequence( Mobile target, bool allowDead )
		{
			if ( !target.Alive && !allowDead )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeBeneficial( target, true, allowDead ) && CheckSequence() )
			{
				Caster.DoBeneficial( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence( Mobile target )
		{
			if ( !target.Alive )
			{
				m_Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				return false;
			}
			else if ( Caster.CanBeHarmful( target ) && CheckSequence() )
			{
				Caster.DoHarmful( target );
				return true;
			}
			else
			{
				return false;
			}
		}

		private class AnimTimer : Timer
		{
			private Spell m_Spell;

			public AnimTimer( Spell spell, int count ) : base( TimeSpan.Zero, AnimateDelay, count )
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.State != SpellState.Casting || m_Spell.m_Caster.Spell != m_Spell )
				{
					Stop();
					return;
				}

				if ( !m_Spell.Caster.Mounted && m_Spell.Caster.Body.IsHuman && m_Spell.m_Info.Action >= 0 )
				{
					m_Spell.Caster.Animate( m_Spell.m_Info.Action, 7, 1, true, false, 0 );
				}

				if ( !Running )
				{
					m_Spell.m_AnimTimer = null;
				}
			}
		}

		private class CastTimer : Timer
		{
			private Spell m_Spell;

			public CastTimer( Spell spell, TimeSpan castDelay ) : base( castDelay )
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if ( m_Spell.m_State == SpellState.Casting && m_Spell.m_Caster.Spell == m_Spell )
				{
					m_Spell.m_State = SpellState.Sequencing;
					m_Spell.m_CastTimer = null;
					m_Spell.m_Caster.OnSpellCast( m_Spell );
					m_Spell.m_Caster.Region.OnSpellCast( m_Spell.m_Caster, m_Spell );
					m_Spell.m_Caster.NextSpellTime = DateTime.Now + m_Spell.GetCastRecovery(); // Spell.NextSpellDelay;

					Target originalTarget = m_Spell.m_Caster.Target;

					m_Spell.OnCast();

					if ( m_Spell.m_Caster.Player && m_Spell.m_Caster.Target != originalTarget && m_Spell.Caster.Target != null )
					{
						m_Spell.m_Caster.Target.BeginTimeout( m_Spell.m_Caster, TimeSpan.FromSeconds( 30.0 ) );
					}

					m_Spell.m_CastTimer = null;
				}
			}
		}
	}
}