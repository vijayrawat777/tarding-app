# ?? BUY/SELL DECISION DATA GUIDE

## Overview

When you click **[BUY]** or **[SELL]** on any option strike, a modal opens showing you everything needed to make a decision.

---

## ?? SECTION 1: Contract Details

### What You'll See
```
Symbol:           NIFTY50 24JanC24900
Strike Price:     ?24,900
Current Price:    ?480
Ask Price:        ?485        ? What you'll PAY to BUY
Bid Price:        ?483.50     ? What you'll GET to SELL
Bid-Ask Spread:   ?1.50 (GOOD)
```

### What It Means

**Symbol**: The exact contract identifier
- Tells you stock (NIFTY50) + expiry (24Jan) + type (C=Call) + strike (24900)

**Strike Price**: Exercise price
- If you buy this CALL at ?24,900 strike
- You can exercise to buy NIFTY50 at ?24,900 (usually doesn't happen)
- But you profit if NIFTY50 goes above ?24,900

**Current Price (LTP)**: Last trade price
- ?480 was the last price someone traded at
- May have changed since you opened the modal

**Ask Price** (For BUYing): ?485
- You must pay ?485 to BUY this contract
- This is what seller is asking for
- Your actual cost = ?485 per contract × quantity

**Bid Price** (For SELLing): ?483.50
- You'll receive ?483.50 if you SELL
- This is what buyer is offering
- Your received amount = ?483.50 × quantity

**Bid-Ask Spread**: ?1.50
- Difference between Bid (?483.50) and Ask (?485)
- Quality Rating:
  - ? EXCELLENT: < 1% of price
  - ? GOOD: < 3% of price
  - ?? FAIR: < 5% of price
  - ? WIDE (AVOID): > 5% of price
- **Why it matters**: 
  - Tight spread = easy to buy AND sell
  - Wide spread = hard to exit position

---

## ?? SECTION 2: Risk Analysis

### For BUYERS

```
Max Loss:           ?485       ? You lose this if it goes to ZERO
Breakeven:          ?25,385    ? Price where you break even
Days to Expiry:     21 days    ? Time left
Time Decay Impact:  -?0.15/day ? Losing this per day
```

**Max Loss (?485)**
- Worst case: You lose entire premium paid
- Why? If NIFTY50 never goes above ?24,900
- Option expires worthless
- You lose ?485 × quantity

**Breakeven (?25,385)**
- For CALLS: Strike + Premium = ?24,900 + ?485 = ?25,385
- For PUTS: Strike - Premium = ?25,000 - ?500 = ?24,500
- If NIFTY50 ends at breakeven:
  - You get your money back (no profit/loss)

**Days to Expiry (21 days)**
- How long until the contract expires
- Why it matters:
  - < 5 days: Risky (less time for move)
  - 5-10 days: Medium risk
  - > 10 days: Safer (more time)

**Time Decay Impact (-?0.15/day)**
- Theta value × number of days
- You lose ?0.15 daily due to time
- Over 21 days = ?3.15 loss just from time!
- Buyers want Theta to be SMALL negative
- Sellers want Theta to be BIG negative

---

### For SELLERS

```
Max Profit:         ?485       ? Best case profit
Risk Level:         MODERATE   ? ITM/OTM distance
Days to Expiry:     21 days    ? Time benefit
Time Decay:         +?0.15/day ? Gaining this per day
```

**Max Profit (?485)**
- Best case: You keep entire premium received
- How? If NIFTY50 never goes below strike
- Option expires worthless (for you = perfect!)
- You keep ?485 × quantity

**Risk Level**
- SAFE (OTM): Strike is > 5% away from current price
  - Low chance of losing money
  - Good for beginners
- MODERATE (ATM): Strike is 2-5% away
  - Decent risk/reward
  - Professional range
- HIGH (ITM): Strike is < 2% away or breached
  - High chance of assignment
  - Very risky!

**Time Decay (+?0.15/day)**
- Good news for sellers!
- You GAIN ?0.15 daily just from time passing
- Over 21 days = ?3.15 free profit!
- Longer expiry = more time decay benefit

---

## ?? SECTION 3: Greeks & Metrics

### DELTA (?) - "How much will price change?"

```
Delta Value:  0.55
Interpretation: Medium Leverage
```

**What it means:**
- If NIFTY50 moves ?1, this option moves ?0.55
- Call Delta = 0.0 to 1.0
- Put Delta = 0.0 to -1.0

**Interpretation Guide:**
- **< 0.3**: Low Leverage
  - Small price movements = small profit/loss
  - Far OTM options
  - Cheap but risky

- **0.3 - 0.7**: Medium Leverage
  - Moderate movement = moderate profit/loss
  - ATM options (best for trading)
  - Balanced risk/reward

- **> 0.7**: High Leverage
  - Large movement = large profit/loss
  - ITM options
  - Expensive but moves a lot

**For Buyers**: Want Delta?
- Bullish on NIFTY? Higher Delta (>0.5)
- Want to save money? Lower Delta (<0.3)

**For Sellers**: Want Delta?
- Selling Calls? Higher Delta = higher assignment risk
- Selling Puts? Lower Delta = safer

---

### GAMMA (?) - "How fast does Delta change?"

```
Gamma Value: 0.0085
```

**What it means:**
- If stock moves ?1, Delta changes by 0.0085
- Higher Gamma = Delta changes faster

**Simple Example:**
- Option has Delta = 0.50, Gamma = 0.01
- Stock moves ?1 up
- New Delta = 0.51 (changed by 0.01)
- Option now moves ?0.51 per ?1 stock move

**Why it matters:**
- **For Buyers**: Want LOW Gamma
  - Predictable behavior
  - Won't swing wildly

- **For Sellers**: Want LOW Gamma
  - Can control positions
  - Won't lose money suddenly

- **High Gamma = Risk!**
  - Position can swing suddenly
  - Can turn profitable to loss fast
  - Avoid if possible

---

### THETA (?) - "Time decay per day"

```
Theta Value: -0.0042
```

**What it means:**
- Option loses ?0.0042 value per day
- Just from time passing!

**For Buyers (-Theta = Bad):**
- Buying CALLS/PUTS? Theta works against you
- Every day, option loses value
- Even if stock doesn't move!
- With -0.0042 Theta:
  - Lose ?0.0042/day × 100 contracts = ?0.42/day
  - Over 21 days = ?8.82 loss just from time!

**For Sellers (+Theta = Good):**
- Selling CALLS/PUTS? Theta works for you!
- Every day, option loses value
- You keep that profit!
- Same +0.0042 Theta:
  - Gain ?0.0042/day × 100 contracts = ?0.42/day
  - Over 21 days = ?8.82 free profit!

**Decision Impact:**
- **Buyers**: Avoid high negative Theta
- **Sellers**: Prefer high positive Theta

---

### VEGA (?) - "Volatility sensitivity"

```
Vega Value: 0.2145
```

**What it means:**
- Option gains/loses ?0.2145 if IV changes by 1%

**Example:**
- IV = 20%, Vega = 0.215
- IV increases to 21% (+1%)
- Option gains ?0.215 value!
- IV decreases to 19% (-1%)
- Option loses ?0.215 value

**For Buyers:**
- If you expect volatility to RISE: Buy
  - IV goes up = option value up = profit!
- If you expect volatility to FALL: Avoid
  - IV goes down = option value down = loss!

**For Sellers:**
- If you expect volatility to FALL: Sell
  - IV goes down = option value down = profit!
- If you expect volatility to RISE: Avoid
  - IV goes up = option value up = loss!

---

### IMPLIED VOLATILITY (IV) - "Market expectation"

```
IV Value: 22%
Quality: NORMAL
```

**What it means:**
- Market expects 22% annualized volatility
- Higher IV = more movement expected
- Lower IV = less movement expected

**Quality Ratings:**
- **LOW (< 18%)**
  - Market thinks: Not much movement
  - Options are CHEAP
  - Best to BUY (cheap)
  - Bad to SELL (low premium)

- **NORMAL (18-25%)**
  - Market thinks: Normal movement
  - Options are FAIR-priced
  - Can buy or sell (no special advantage)

- **HIGH (> 25%)**
  - Market thinks: Lots of movement expected
  - Options are EXPENSIVE
  - Bad to BUY (overpriced)
  - Best to SELL (high premium)

**Decision Help:**
- IV < 18% ? BUY options (cheap)
- IV > 25% ? SELL options (get good premium)
- IV 18-25% ? Your preference

---

## ?? SECTION 4: Liquidity

### VOLUME - "How many traded today"

```
Volume: 35.5K
Quality: GOOD
```

**What it means:**
- 35,500 contracts traded so far today
- Shows market activity

**Quality Ratings:**
- **EXCELLENT (> 50K)**
  - Very liquid
  - Easy to buy AND sell
  - Best for trading
  - Tight spreads likely

- **GOOD (> 20K)**
  - Liquid enough
  - Can buy and sell
  - Spreads OK
  - Standard for most

- **FAIR (> 5K)**
  - Barely liquid
  - Can trade but harder
  - Wider spreads
  - Avoid if possible

- **LOW (< 5K)**
  - Not liquid!
  - Hard to exit position
  - Very wide spreads
  - AVOID! (You might get stuck)

**Why it matters:**
- Buy LOW volume = Can't sell easily = Get stuck
- Sell LOW volume = Can't exit = Risk of loss

---

### OPEN INTEREST (OI) - "Total outstanding"

```
OI: 285K
```

**What it means:**
- 285,000 contracts still open (not exercised/closed)
- Shows contract popularity

**Interpretation:**
- High OI = Popular contract
  - Liquidity should be good
  - Prices stable

- Low OI = Unpopular contract
  - May be hard to trade
  - Wide spreads
  - Avoid!

**Volume vs OI:**
- **High Volume + High OI** = PERFECT
  - Easy entry and exit
  - Tight spreads

- **Low Volume + Low OI** = DANGEROUS
  - Hard to exit
  - Wide spreads
  - You get stuck!

---

## ?? SECTION 5: Buy/Sell Decision Guide

### BUY DECISION

**? BUY IF:**
```
? You're BULLISH (expect NIFTY to go UP)
? IV is LOW (options are cheap)
? Volume is GOOD (can exit if wrong)
? Ready to lose entire premium paid
? Have more than 5 days until expiry
```

**? AVOID IF:**
```
? You're BEARISH (expect NIFTY to go DOWN)
? IV is HIGH (options are expensive)
? Volume is LOW (can't exit)
? Can't afford to lose the premium
? Less than 5 days until expiry
```

### SELL DECISION

**? SELL IF:**
```
? You're BEARISH/NEUTRAL (don't expect big up)
? IV is HIGH (collect good premium)
? Have margin available (for worst case)
? Volume is GOOD (can exit if wrong)
? Strike is OTM (safe distance)
```

**? AVOID IF:**
```
? You're BULLISH (expect big upside)
? Gamma is HIGH (sudden loss risk)
? Less than 3 days until expiry
? Can't afford maximum loss
? Strike is ITM (assignment risk)
? No margin available
```

---

## ?? Quick Decision Checklists

### Before BUYING Call Option

- [ ] Do I expect stock to go UP?
- [ ] Is IV LOW or NORMAL?
- [ ] Is Volume at least GOOD?
- [ ] Can I lose the entire premium?
- [ ] Is there > 5 days to expiry?
- [ ] Is Delta reasonable (0.3-0.7)?
- [ ] Is spread GOOD or better?

If ANY box is unchecked ? **DON'T BUY**

### Before BUYING Put Option

- [ ] Do I expect stock to go DOWN?
- [ ] Is IV LOW or NORMAL?
- [ ] Is Volume at least GOOD?
- [ ] Can I lose the entire premium?
- [ ] Is there > 5 days to expiry?
- [ ] Is Delta reasonable (-0.7 to -0.3)?
- [ ] Is spread GOOD or better?

If ANY box is unchecked ? **DON'T BUY**

### Before SELLING Call Option

- [ ] Do I expect stock NEUTRAL or DOWN?
- [ ] Is IV HIGH?
- [ ] Do I have margin?
- [ ] Is Volume at least GOOD?
- [ ] Is strike OTM or ATM (not ITM)?
- [ ] Is Gamma not too high?
- [ ] Is there > 3 days to expiry?

If ANY box is unchecked ? **DON'T SELL**

### Before SELLING Put Option

- [ ] Do I expect stock NEUTRAL or UP?
- [ ] Is IV HIGH?
- [ ] Do I have margin?
- [ ] Is Volume at least GOOD?
- [ ] Is strike OTM or ATM (not ITM)?
- [ ] Is Gamma not too high?
- [ ] Is there > 3 days to expiry?

If ANY box is unchecked ? **DON'T SELL**

---

## ?? Common Questions

### "What does Delta 0.60 mean?"
- Stock moves ?1 ? Option moves ?0.60
- 60% correlation with stock
- Medium leverage

### "Should I buy high IV options?"
- NO! They're expensive
- Buy when IV is LOW (cheap)
- Sell when IV is HIGH (expensive premium)

### "What if volume is LOW?"
- You can't exit easily
- You get stuck with position
- Avoid LOW volume options!

### "Time decay helps or hurts?"
- HURTS buyers (Theta negative)
- HELPS sellers (Theta positive)
- Both feel it over 21 days

### "Why check Gamma?"
- Helps predict sudden moves
- High Gamma = unpredictable swings
- Low Gamma = stable behavior

---

## ? Summary

### Before Clicking Confirm

**Always check:**
1. ? Right direction? (Bullish/Bearish/Neutral)
2. ? Right price? (IV cheap/expensive?)
3. ? Right volume? (Can I exit?)
4. ? Right risk? (Can I lose this much?)
5. ? Right time? (Enough days until expiry?)

**If everything checks out** ? Click Confirm!

---

**This modal gives you EVERYTHING needed to make a smart decision!**

Read it carefully. Don't skip sections. Make informed choices. ??
