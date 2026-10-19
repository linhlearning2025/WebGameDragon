# EXECUTION_GUIDE — Làm từng ticket bằng GitHub Spec Kit và Copilot

**Nguồn sự thật:** [`TICKET_INDEX.md`](TICKET_INDEX.md) để lấy thứ tự; `metadata/implementation_order.json` để script đọc; file ticket trong `tickets/` để lấy WHAT/AC. Không paste tất cả 58 tickets vào một lệnh Spec Kit.

## 1. Đặt bộ backlog trong repo

Sao chép nguyên `Dragon_English_Kids_SDD_Backlog/` vào `docs/sdd/Dragon_English_Kids_SDD_Backlog/` (hoặc đổi root nhưng giữ path tương đối). Khởi tạo Spec Kit tại **root repository** trước, chọn GitHub Copilot trong VS Code. Phê duyệt `CONSTITUTION.md` trước khi implement; `OPEN_DECISIONS.md` liệt kê các quyết định chưa chốt.

## 2. Lấy ticket tiếp theo

```bash
python docs/sdd/Dragon_English_Kids_SDD_Backlog/tools/validate_backlog.py
python docs/sdd/Dragon_English_Kids_SDD_Backlog/tools/next_ticket.py
```

`next_ticket.py` chỉ dựa vào dependency và trạng thái; nó **không** tự duyệt business decisions. Mặc định tất cả ở `TODO`, ticket đầu tiên là `PLAT-001`.

## 3. Copy prompt cho Copilot Agent

Ví dụ cho ticket đầu tiên:

```text
/speckit-specify

Feature: PLAT-001 — Monorepo Foundation for Dragon English Kids.
Read: docs/sdd/Dragon_English_Kids_SDD_Backlog/tickets/platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md
Read also: docs/sdd/Dragon_English_Kids_SDD_Backlog/CONSTITUTION.md and relevant decisions.
Preserve original ticket ID PLAT-001, business requirements and Given/When/Then acceptance criteria.
Document ambiguities without inventing unapproved product policies.
Generate spec only; do not implement code yet.
```

Copilot CLI/slash command names vary by Spec Kit version (`/speckit-specify` versus `/speckit.specify`). Select the actual installed command in VS Code. After specification: Clarify → Plan → Tasks → Analyze → Implement → tests → review. Tasks in the SDD ticket are suggestions; Spec Kit owns the actual `specs/<feature>/tasks.md`.

## 4. Complete ticket and unlock the next

Set `metadata/progress.json` value from `TODO` to `DONE` only after acceptance tests, CI, contracts, PR review and DoD. Supported states: `TODO`, `IN_PROGRESS`, `BLOCKED`, `DONE`.

```json
"PLAT-001": { "status": "DONE", "pr": "#42", "notes": "AC and CI verified" }
```

Run `next_ticket.py` again. It prints the next valid sequential ticket as well as additional READY tickets that may be worked on in parallel.

## 5. Milestone gates and change management

Before starting M2, confirm M1 guest-to-map flow and sign off critical decisions; before public launch, confirm child privacy/consent, licensed content, audio performance, security and data-deletion flows. Any new blocking dependency must update **ticket frontmatter + dependency section + tickets.json/csv + order/index + validation**, not just prose.
