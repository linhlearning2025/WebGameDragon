---
id: PLAT-001
title: "Thiết lập monorepo và môi trường phát triển"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: platform
dependencies: []
source: "PRD §1; Tech Stack"
stack: [react, phaser-3, dotnet, postgresql]
---

# PLAT-001 — Thiết lập monorepo và môi trường phát triển

> **Spec-driven ticket** · Domain: **Foundation & Architecture** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là developer, tôi cần khởi chạy đầy đủ React, Phaser và .NET trong môi trường local bằng quy trình lặp lại được.

**Nguồn:** PRD §1; Tech Stack. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Tách web app, API, tools tạo học liệu; đồng bộ phiên bản và scripts build/test.
- Cấu hình PostgreSQL và mock storage/local assets; secrets không commit.
- Phải có healthcheck, lint, test script và hướng dẫn setup từ máy sạch.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given máy dev mới có toolchain đã nêu
When chạy hướng dẫn setup
Then React và API hoạt động với cấu hình development.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given missing env bắt buộc
When khởi động
Then ứng dụng fail fast với thông báo tên biến (không lộ giá trị).
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given pull request
When CI chạy
Then build/lint/test tối thiểu được kiểm tra.
```

## 4. Contract & Data Impact

- Repo `/apps/web`, `/apps/api`, `/tools/content-pipeline` (đề xuất).
- Env: DATABASE_URL, R2_* và provider keys dùng secret store.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Không có DB: API trả readiness fail, không giả định DB sẵn sàng.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Smoke test startup, CI checks và secret scanning.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Tạo projects và package scripts
- [ ] Cấu hình Docker Compose Postgres
- [ ] Cài lint/test và CI
- [ ] Viết quickstart.

## 8. Dependencies & Boundaries

**Blocked by:** Không có ticket chặn bắt buộc.

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt monorepo hay multirepo; phiên bản .NET/Node/Phaser cần pin.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
