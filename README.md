<h1>
  <img src="./poker.svg.svg" width=32 height="32" alt="">
  PokerOdds
</h1>

A .NET application that automatically analyzes a live poker table from screen captures. It recognizes cards, evaluates the current hand and estimates the probability of winning against opponents.

## Flow

```text
Poker Table
     ↓
Screen Capture
     ↓
Image Processing
     ↓
OCR / Template Matching
     ↓
Card Recognition
     ↓
Monte Carlo Simulation
     ↓
┌───────────────────────────────┐
│       Simulation Loop         │
│                               │
│  Complete Board               │
│       ↓                       │
│  Generate Opponent Hands      │
│       ↓                       │
│  Evaluate Hands               │
│       ↓                       │
│  Compare Results              │
└───────────────────────────────┘
     ↓
Win Probability
     ↓
Pot Odds
     ↓
Decision
```

## Implementation

Built with **ASP.NET Core Web API**, **Tesseract OCR** and **Emgu CV** for card recognition, **Monte Carlo simulation** for win probability estimation, and **parallel processing** for concurrent simulations.
