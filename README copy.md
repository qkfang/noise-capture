# Claims Team in a Day

A demo for **Zava Insurance** that showcases an AI-powered claims operation as a coordinated team of specialised agents. Inside the Zava Insurance claims office, each department — intake, assessment, loss adjusting, fraud, supplier coordination, settlement, customer communications, and team leadership — is represented by its own AI agent supporting human staff through the full claims lifecycle.

See [AGENTS.md](AGENTS.md) for the demo overview and the [docs/](docs/) folder for characters, process, scenarios, and visual themes.

---

## The Claims Agent Team

Each agent owns a specific part of the claim lifecycle and shares a common `Claim Case` record. Agents support — they do not replace — human judgement, and human approval is required for sensitive decisions such as declines, high-value settlements, fraud referrals, and bereavement communications.

| # | Agent | Staff Role | What it does |
|---|-------|------------|--------------|
| 1 | **Claims Intake Agent** (Iris) | Claims Intake Officer | Captures first notice of loss, classifies the claim, checks policy status, and requests missing documents. |
| 2 | **Claims Assessment Agent** (Adam) | Claims Assessor | Reads the policy, checks evidence and exclusions, and recommends approve / partial / decline with reasoning. |
| 3 | **Loss Adjuster Agent** (Lara) | Loss Adjuster | Investigates complex or high-value losses — scope of damage, cost reasonableness, and inspection briefs. |
| 4 | **Fraud Investigation Agent** (Felix) | Fraud Investigator | Surfaces timeline inconsistencies, scores risk explainably, and runs per-document authenticity checks. |
| 5 | **Supplier Coordinator Agent** (Sam) | Supplier Coordinator | Picks the right repairer, books appointments, compares quotes, and chases overdue suppliers. |
| 6 | **Settlement Agent** (Seth) | Settlement Officer | Calculates the payable amount — limits, excess, depreciation, prior payments — and prepares the payment. |
| 7 | **Customer Communications Agent** (Cara) | Customer Communications Specialist | Drafts plain-English, empathetic updates, requests for information, and decision letters. |
| 8 | **Team Leader Agent** (Theo) | Claims Team Leader | Watches the whole floor — workload, SLA, escalations, approvals, and quality. |

### How a claim flows through the team

1. **Intake** creates the claim and requests required documents.
2. **Assessment** reviews policy and evidence and proposes a decision.
3. **Loss Adjusting** runs for complex or high-value damage.
4. **Fraud Investigation** runs when risk indicators are detected.
5. **Supplier Coordination** books and tracks third parties.
6. **Settlement** calculates and prepares payment.
7. **Customer Communications** keeps the customer informed at every step.
8. **Team Leader** monitors the whole workflow and routes escalations.

For full agent capabilities, inputs, tools, outputs, and human-approval rules, see [docs/foundry_agents.md](docs/foundry_agents.md).

---

## Screenshots

### Backend — Agent operations console

The backend hub lists every agent in the claims office. Each card opens an agent profile with two real customer scenarios you can step through.

![Backend agent console](docs/res/app-backend.png)

### Frontend — Voxel claims office

The frontend renders the Zava Insurance claims office as a voxel, isometric workplace. Customers arrive at reception, claims move between departments, and the live activity feed shows what each agent is doing right now.

![Frontend voxel claims office](docs/res/app-frontend.png)

---

## Contributing

Contributions are very welcome! Whether you want to add a new agent, refine an existing one, improve a scenario, polish the voxel office, or fix a typo — we'd love your help.

Ways to contribute:

- **Open an issue** to report a bug, suggest an enhancement, or propose a new claim scenario.
- **Submit a pull request** with code, content, or documentation changes — small PRs are perfect.
- **Share ideas** for new agents, customer personas, or claims workflows that fit the Zava Insurance story.
- **Improve the docs** in the [docs/](docs/) folder — characters, process, scenarios, and visual themes.

Please keep contributions aligned with the claims-office metaphor and the guidance in [AGENTS.md](AGENTS.md). Be kind, be constructive, and have fun building the claims team.