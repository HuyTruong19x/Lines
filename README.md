# Lines

***Luật chơi*** :
- Luật chơi dựa trên trò line98

***Các loại bóng*** :
- Bóng bình thường : chỉ chứa màu sắc
- Bóng ma : có thể di chuyển khắp nơi mà không bị chặn lại bởi quả bóng khác
- Bóng cầu vồng : Sau khi di chuyển, biến đổi ngẫu nhiên thành 1 màu bất kì, hãy cực kì cận thận với nó. Đôi khi nó mang lại lợi ích cho bạn hoặc không
- Bóng sọc ngang : Sau khi di chuyển, biến đổi 2 quả bóng bên cạnh theo chiều ngang thành cùng màu với chính nó

***Chế độ AR (AR scene)*** :
- Đảm bảo device có hỗ trợ AR, kiểm tra tại : https://developers.google.com/ar/devices
- Chuyển sang AR scene bằng cách nhấn vào nút "TO AR SCENE" ở scene đầu tiên và cho phép ứng dụng sử dụng camera của bạn.
- Game sẽ xác định mặt phẳng ngang bằng camera, sau đó bạn có thể chọn bất cứ mặt phẳng nào để hiện thị game trên đó
- Khi tắt chế độ AR, game sẽ tự động chuyển về chế độ chơi thường ( vẫn giữ nguyên trạng thái trò chơi)
- Vuốt trái/phải để xoay game board trong chế độ AR


***Cấu hình game***

- Chỉnh sửa thông số tại Assets\Resources\ScriptAbleObject\Basic Level

- Hoặc tạo mới : Create -> Game Setting -> New Game Setting và đặt nó vào GameManager