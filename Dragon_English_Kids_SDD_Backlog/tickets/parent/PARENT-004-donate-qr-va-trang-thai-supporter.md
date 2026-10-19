---
id: PARENT-004
title: "Donate QR và trạng thái supporter"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P2
milestone: M3
domain: parent
dependencies: [AUTH-005, AUTH-001]
source: "PRD US-1.5; users.is_supporter"
stack: [react, phaser-3, dotnet, postgresql]
---

# PARENT-004 — Donate QR và trạng thái supporter

> **Spec-driven ticket** · Domain: **Parent Dashboard & Donation** · Priority: **P2** · Release: **M3** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là phụ huynh, tôi muốn xem mã QR ủng hộ dự án và nhận xác nhận đúng cách.

**Nguồn:** PRD US-1.5; users.is_supporter. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- QR chỉ hiển thị sau parental gate; nêu rõ tự nguyện và không ảnh hưởng nội dung học cốt lõi.
- Không đánh dấu `is_supporter=true` khi chỉ mở/scan QR; cần xác thực giao dịch qua kênh được chốt.
- Không để trẻ bị thúc ép thanh toán hoặc hiển thị upsell trong scene học tập.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given parent mở Donate
When hiện QR
Then hiển thị ngân hàng/người nhận/nội dung rõ từ server config.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given chỉ quét QR
When chưa có payment confirmation
Then is_supporter=false.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given admin xác nhận donation
When cập nhật
Then supporter entitlement được cấp một lần.
```

## 4. Contract & Data Impact

- Proposed `GET /api/v1/parent/donate/qr`, payment confirmation manual/admin workflow; donor ledger.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Không auto xác nhận từ screenshot/claim của client.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Authorization, supporter false on scan, duplicate confirmation, ledger audit.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Donate UI
- [ ] QR config
- [ ] Confirmation workflow
- [ ] Tests.

## 8. Dependencies & Boundaries

**Blocked by:** [AUTH-005](../identity/AUTH-005-parental-gate-voi-pin-bam-va-xac-thuc.md), [AUTH-001](../identity/AUTH-001-ang-ky-ang-nhap-phu-huynh-bang-email.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Cần chốt nhà cung cấp payment/webhook hay đối soát thủ công; QR không đủ để xác minh tiền.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
