const FOOD_GUIDE_ITEMS = [
  {
    id: 1,
    nameVi: 'Rau cải xanh',
    nameEn: 'Mustard Greens',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/rau-cai-xanh.jpg',
    shortDesc: 'Rau lá xanh thường dùng để luộc, xào hoặc nấu canh.',
    chooseTips: ['Chọn lá xanh tự nhiên, không héo úa.', 'Thân rau giòn, không bị nhớt.', 'Bó rau có mùi rau tươi, không mùi lạ.'],
    avoidSigns: ['Tránh lá úa vàng, dập nát hoặc thối nhũn.', 'Không chọn rau có đốm mốc hoặc nhớt.', 'Tránh bó rau có mùi hóa chất nồng bất thường.'],
    storage: ['Nhặt bỏ lá hư.', 'Bọc giấy hoặc túi thoáng rồi để ngăn mát.', 'Không rửa trước nếu chưa dùng ngay.'],
    safetyNote: 'Rửa kỹ nhiều lần trước khi chế biến. Nấu chín nếu dùng cho trẻ nhỏ, người già hoặc người có hệ tiêu hóa nhạy cảm.'
  },
  {
    id: 2,
    nameVi: 'Rau muống',
    nameEn: 'Water Spinach',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/rau-muong.jpg',
    shortDesc: 'Rau phổ biến để luộc, xào tỏi hoặc nấu canh chua.',
    chooseTips: ['Chọn cọng vừa, thân giòn và lá xanh.', 'Cọng không quá già hoặc xơ.', 'Rau không bị dập nhiều ở phần ngọn.'],
    avoidSigns: ['Tránh rau nhớt, thâm đen hoặc có mùi lạ.', 'Không chọn rau héo rũ, lá vàng nhiều.', 'Tránh rau có vết sâu hỏng lan rộng.'],
    storage: ['Để rau khô ráo trong túi thoáng.', 'Bảo quản ngăn mát và dùng trong 1-2 ngày.', 'Không để rau cạnh thịt cá sống chưa đóng gói.'],
    safetyNote: 'Rửa dưới vòi nước, ngâm rửa kỹ phần cọng. Nấu chín trước khi ăn nếu không chắc nguồn gốc.'
  },
  {
    id: 3,
    nameVi: 'Cà chua',
    nameEn: 'Tomato',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/ca-chua.jpg',
    shortDesc: 'Loại quả phổ biến, thường dùng trong món canh, sốt, salad và món xào.',
    chooseTips: ['Chọn quả có màu đỏ tự nhiên, vỏ căng, không bị nhăn.', 'Cầm thấy chắc tay, không quá mềm.', 'Cuống còn xanh nhẹ là dấu hiệu cà chua còn tương đối tươi.'],
    avoidSigns: ['Tránh quả bị dập, nứt, chảy nước.', 'Tránh quả có mùi chua lạ hoặc mốc.', 'Không chọn quả bị thâm đen hoặc mềm nhũn.'],
    storage: ['Bảo quản nơi thoáng mát nếu dùng trong 1-2 ngày.', 'Nếu cà chua đã chín mềm, có thể để ngăn mát tủ lạnh.', 'Không để chung với thực phẩm sống như thịt cá chưa đóng gói.'],
    safetyNote: 'Rửa sạch dưới vòi nước trước khi cắt. Không sử dụng nếu có dấu hiệu mốc, chảy nước hoặc mùi lạ.'
  },
  {
    id: 4,
    nameVi: 'Cà rốt',
    nameEn: 'Carrot',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/ca-rot.jpg',
    shortDesc: 'Củ giòn ngọt, dùng cho món canh, xào, salad hoặc nước ép.',
    chooseTips: ['Chọn củ màu cam đều, cầm chắc tay.', 'Vỏ tương đối mịn, không nứt sâu.', 'Đầu củ không bị mềm hoặc thối.'],
    avoidSigns: ['Tránh củ mềm nhũn, mốc hoặc chảy nước.', 'Không chọn củ có mùi lạ.', 'Tránh củ quá khô teo nếu muốn ăn sống hoặc làm salad.'],
    storage: ['Cắt bỏ phần lá nếu còn.', 'Bọc kín vừa phải rồi để ngăn mát.', 'Giữ khô ráo để hạn chế mốc.'],
    safetyNote: 'Rửa và gọt sạch vỏ nếu cần. Không dùng phần bị mốc lan vào trong củ.'
  },
  {
    id: 5,
    nameVi: 'Khoai tây',
    nameEn: 'Potato',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/khoai-tay.jpg',
    shortDesc: 'Củ giàu tinh bột, dùng để hầm, chiên, nghiền hoặc nấu soup.',
    chooseTips: ['Chọn củ chắc, vỏ khô, không bị ẩm nhớt.', 'Bề mặt không có nhiều vết thâm sâu.', 'Củ không mọc mầm là lựa chọn tốt hơn.'],
    avoidSigns: ['Tránh khoai mọc mầm nhiều hoặc vỏ xanh.', 'Không dùng củ mềm nhũn, mốc hoặc có mùi lạ.', 'Cắt bỏ phần hỏng nhỏ, nhưng bỏ cả củ nếu hỏng nhiều.'],
    storage: ['Để nơi tối, thoáng, khô.', 'Không để gần hành tây lâu ngày.', 'Không rửa trước khi cất.'],
    safetyNote: 'Không ăn khoai tây xanh hoặc mọc mầm nhiều vì có thể gây khó chịu tiêu hóa.'
  },
  {
    id: 6,
    nameVi: 'Bí đỏ',
    nameEn: 'Pumpkin',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/bi-do.jpg',
    shortDesc: 'Bí vị ngọt nhẹ, dùng cho canh, soup, cháo hoặc món hấp.',
    chooseTips: ['Chọn quả hoặc miếng bí có màu vàng cam tự nhiên.', 'Thịt bí chắc, không bị mềm nhũn.', 'Vỏ không có mốc hoặc vết thối lan rộng.'],
    avoidSigns: ['Tránh bí chảy nước, nhớt hoặc có mùi chua.', 'Không chọn miếng cắt bị khô đen quá nhiều.', 'Tránh phần ruột bị mốc.'],
    storage: ['Bí nguyên quả để nơi thoáng mát.', 'Bí đã cắt nên bọc kín và để ngăn mát.', 'Dùng phần đã cắt trong vài ngày.'],
    safetyNote: 'Rửa sạch vỏ trước khi cắt để tránh bẩn từ vỏ bám vào thịt bí.'
  },
  {
    id: 7,
    nameVi: 'Cam',
    nameEn: 'Orange',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/cam.jpg',
    shortDesc: 'Trái cây mọng nước, dùng ăn trực tiếp hoặc ép nước.',
    chooseTips: ['Chọn quả cầm nặng tay so với kích thước.', 'Vỏ căng, không quá khô héo.', 'Mùi thơm nhẹ tự nhiên ở phần vỏ.'],
    avoidSigns: ['Tránh quả mốc, mềm nhũn hoặc chảy nước.', 'Không chọn quả có mùi lên men.', 'Tránh vỏ bị thâm lan rộng.'],
    storage: ['Để nơi thoáng nếu dùng sớm.', 'Bảo quản ngăn mát để giữ lâu hơn.', 'Không để cam đã cắt ở nhiệt độ phòng quá lâu.'],
    safetyNote: 'Rửa sạch vỏ trước khi cắt hoặc vắt để hạn chế bụi bẩn đi vào nước cam.'
  },
  {
    id: 8,
    nameVi: 'Táo',
    nameEn: 'Apple',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/tao.jpg',
    shortDesc: 'Trái cây giòn ngọt, dễ mang theo và dùng làm món ăn nhẹ.',
    chooseTips: ['Chọn quả chắc tay, vỏ căng.', 'Màu sắc đều theo giống táo.', 'Cuống không bị mốc hoặc chảy nước.'],
    avoidSigns: ['Tránh quả bầm mềm, thối hoặc có mùi lạ.', 'Không chọn quả nứt sâu.', 'Tránh phần lõm bị mốc quanh cuống.'],
    storage: ['Để ngăn mát nếu muốn giữ độ giòn.', 'Tách khỏi trái quá chín nếu muốn bảo quản lâu.', 'Táo đã cắt nên dùng sớm hoặc bọc kín.'],
    safetyNote: 'Rửa kỹ dưới vòi nước trước khi ăn, đặc biệt nếu ăn cả vỏ.'
  },
  {
    id: 9,
    nameVi: 'Chuối',
    nameEn: 'Banana',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/chuoi.jpg',
    shortDesc: 'Trái cây mềm, ngọt, phù hợp bữa phụ hoặc làm sinh tố.',
    chooseTips: ['Chọn nải chuối có độ chín phù hợp nhu cầu.', 'Vỏ vàng đều, có ít đốm nâu là chuối chín ngọt.', 'Quả còn nguyên, không dập nát nhiều.'],
    avoidSigns: ['Tránh chuối mốc ở cuống.', 'Không chọn quả chảy nước hoặc mùi lên men mạnh.', 'Tránh quả bị thâm đen mềm nhũn toàn bộ.'],
    storage: ['Để nhiệt độ phòng nếu dùng trong vài ngày.', 'Không để chuối gần thực phẩm có mùi mạnh.', 'Chuối chín quá có thể bóc vỏ, cấp đông làm sinh tố.'],
    safetyNote: 'Rửa tay sau khi bóc vỏ nếu vỏ bẩn. Không dùng quả có mốc lan vào thịt chuối.'
  },
  {
    id: 10,
    nameVi: 'Bơ',
    nameEn: 'Avocado',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/bo.jpg',
    shortDesc: 'Trái béo nhẹ, dùng làm sinh tố, salad hoặc ăn kèm bánh mì.',
    chooseTips: ['Chọn quả cầm chắc, hơi mềm nhẹ khi chín.', 'Vỏ không bị nứt sâu hoặc mốc.', 'Cuống không có mùi lạ.'],
    avoidSigns: ['Tránh quả mềm nhũn, chảy nước.', 'Không chọn quả có nhiều vết thâm sâu.', 'Tránh bơ có mùi chua hoặc lên men.'],
    storage: ['Bơ chưa chín để nhiệt độ phòng.', 'Bơ chín để ngăn mát và dùng sớm.', 'Bơ đã cắt nên bọc kín, có thể thêm ít chanh để hạn chế thâm.'],
    safetyNote: 'Rửa vỏ trước khi cắt để tránh bụi bẩn bám vào dao rồi đi vào phần thịt.'
  },
  {
    id: 11,
    nameVi: 'Dưa hấu',
    nameEn: 'Watermelon',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/dua-hau.jpg',
    shortDesc: 'Trái nhiều nước, dùng tráng miệng hoặc ép nước.',
    chooseTips: ['Chọn quả chắc, vỏ lành lặn.', 'Phần tiếp đất có màu vàng kem thường là dấu hiệu chín.', 'Gõ nghe âm trầm vừa, không quá rỗng.'],
    avoidSigns: ['Tránh quả nứt, chảy nước hoặc mềm bất thường.', 'Không mua miếng cắt sẵn để ngoài lâu không che chắn.', 'Tránh mùi chua hoặc lên men.'],
    storage: ['Dưa nguyên quả để nơi thoáng.', 'Dưa đã cắt phải bọc kín và để ngăn mát.', 'Dùng dưa đã cắt trong thời gian ngắn.'],
    safetyNote: 'Rửa sạch vỏ trước khi bổ. Dao và thớt cần sạch vì phần ruột ăn trực tiếp.'
  },
  {
    id: 12,
    nameVi: 'Xoài',
    nameEn: 'Mango',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/xoai.jpg',
    shortDesc: 'Trái cây thơm ngọt hoặc chua nhẹ, dùng ăn trực tiếp, làm gỏi hoặc sinh tố.',
    chooseTips: ['Chọn quả có mùi thơm nhẹ ở cuống.', 'Cầm chắc tay, độ mềm phù hợp độ chín mong muốn.', 'Vỏ không có mốc hoặc vết thối sâu.'],
    avoidSigns: ['Tránh quả chảy nhựa nhiều, mềm nhũn.', 'Không chọn quả có mùi lên men.', 'Tránh mảng thâm đen lan rộng.'],
    storage: ['Xoài xanh hoặc chưa chín để nhiệt độ phòng.', 'Xoài chín để ngăn mát và dùng sớm.', 'Xoài đã cắt phải bọc kín.'],
    safetyNote: 'Rửa vỏ trước khi gọt. Không dùng phần thịt bị mốc hoặc có vị lạ.'
  },
  {
    id: 13,
    nameVi: 'Thịt heo',
    nameEn: 'Pork',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-heo.jpg',
    shortDesc: 'Nguồn đạm phổ biến cho món kho, luộc, xào và nướng.',
    chooseTips: ['Chọn thịt có màu hồng tươi hoặc đỏ nhạt tự nhiên.', 'Bề mặt thịt hơi khô, không nhớt.', 'Khi ấn nhẹ, thịt có độ đàn hồi.'],
    avoidSigns: ['Tránh thịt có màu tái xanh, xám hoặc thâm đen.', 'Không chọn thịt có mùi hôi, mùi chua.', 'Tránh bề mặt nhớt hoặc chảy dịch bất thường.'],
    storage: ['Để ngăn mát nếu dùng trong ngày.', 'Đông lạnh nếu chưa dùng ngay.', 'Không để thịt sống tiếp xúc với rau củ ăn liền.'],
    safetyNote: 'Thịt heo cần nấu chín kỹ trước khi ăn. Nên dùng dao/thớt riêng cho thịt sống.'
  },
  {
    id: 14,
    nameVi: 'Thịt bò',
    nameEn: 'Beef',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-bo.jpg',
    shortDesc: 'Thịt đỏ giàu đạm, dùng cho món xào, hầm, áp chảo hoặc phở.',
    chooseTips: ['Chọn thịt đỏ tươi tự nhiên, thớ thịt rõ.', 'Mỡ bò có màu vàng nhạt, không xám.', 'Bề mặt khô ráo, đàn hồi khi ấn.'],
    avoidSigns: ['Tránh thịt có mùi chua hoặc hôi.', 'Không chọn miếng thịt nhớt, thâm đen.', 'Tránh thịt chảy dịch nhiều bất thường.'],
    storage: ['Bảo quản lạnh ngay sau khi mua.', 'Chia phần nhỏ trước khi cấp đông.', 'Rã đông trong ngăn mát thay vì để ngoài quá lâu.'],
    safetyNote: 'Nấu chín phù hợp món ăn. Người nhạy cảm nên ưu tiên thịt chín kỹ.'
  },
  {
    id: 15,
    nameVi: 'Thịt gà',
    nameEn: 'Chicken',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-ga.jpg',
    shortDesc: 'Nguồn đạm quen thuộc cho món luộc, kho, chiên, nướng và salad.',
    chooseTips: ['Chọn gà có da màu tự nhiên, không bầm tím nhiều.', 'Thịt săn, không có mùi lạ.', 'Bao bì còn nguyên nếu mua trong siêu thị.'],
    avoidSigns: ['Tránh gà nhớt, chảy dịch, mùi hôi.', 'Không chọn phần thịt tái xám hoặc thâm đen.', 'Tránh sản phẩm quá hạn hoặc bao bì phồng.'],
    storage: ['Để ngăn mát nếu dùng trong ngày.', 'Cấp đông nếu chưa dùng ngay.', 'Đặt trong hộp kín để nước thịt không chảy sang thực phẩm khác.'],
    safetyNote: 'Thịt gà phải nấu chín kỹ, không còn màu hồng ở phần dày hoặc gần xương.'
  },
  {
    id: 16,
    nameVi: 'Sườn heo',
    nameEn: 'Pork Ribs',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/suon-heo.jpg',
    shortDesc: 'Phần thịt có xương, hợp nấu canh, kho, nướng hoặc hầm.',
    chooseTips: ['Chọn sườn màu hồng nhạt, xương không thâm đen.', 'Thịt bám xương chắc, bề mặt không nhớt.', 'Mùi thịt tươi tự nhiên.'],
    avoidSigns: ['Tránh sườn có mùi hôi hoặc chua.', 'Không chọn miếng chảy dịch nhiều.', 'Tránh thịt xám, xanh hoặc bầm bất thường.'],
    storage: ['Bảo quản lạnh ngay sau khi mua.', 'Chia phần trước khi đông lạnh.', 'Không để sườn sống gần thực phẩm ăn liền.'],
    safetyNote: 'Nấu chín kỹ, đặc biệt phần thịt sát xương.'
  },
  {
    id: 17,
    nameVi: 'Cá hồi',
    nameEn: 'Salmon',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/ca-hoi.jpg',
    shortDesc: 'Cá béo, thường dùng áp chảo, nướng hoặc làm salad chín.',
    chooseTips: ['Chọn miếng cá màu cam hồng tự nhiên.', 'Thịt cá săn, bề mặt ẩm nhưng không nhớt.', 'Mùi tanh nhẹ tự nhiên, không hôi gắt.'],
    avoidSigns: ['Tránh cá xỉn màu, nhớt nhiều.', 'Không chọn miếng có mùi chua hoặc amoniac.', 'Tránh cá chảy dịch đục.'],
    storage: ['Giữ lạnh liên tục.', 'Dùng trong ngày nếu mua tươi.', 'Cấp đông nếu chưa chế biến ngay.'],
    safetyNote: 'Cá cần được làm sạch và nấu chín kỹ nếu không dùng loại đạt chuẩn ăn sống.'
  },
  {
    id: 18,
    nameVi: 'Cá basa',
    nameEn: 'Basa Fish',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/ca-basa.jpg',
    shortDesc: 'Cá thịt mềm, hợp kho, chiên hoặc nấu canh chua.',
    chooseTips: ['Chọn cá hoặc phi lê có màu sáng tự nhiên.', 'Thịt cá đàn hồi nhẹ.', 'Mùi tanh nhẹ, không hôi gắt.'],
    avoidSigns: ['Tránh cá mềm nhũn, nhớt nhiều.', 'Không chọn cá có màu lạ hoặc mùi chua.', 'Tránh bao bì rách nếu mua đông lạnh.'],
    storage: ['Bảo quản lạnh hoặc đông lạnh.', 'Không rã đông rồi cấp đông lại nhiều lần.', 'Dùng sớm sau khi rã đông.'],
    safetyNote: 'Rửa sạch và nấu chín kỹ. Tách riêng dao/thớt cá sống với thực phẩm chín.'
  },
  {
    id: 19,
    nameVi: 'Tôm',
    nameEn: 'Shrimp',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/tom.jpg',
    shortDesc: 'Hải sản phổ biến cho món luộc, hấp, xào, nướng và canh.',
    chooseTips: ['Chọn tôm vỏ trong, thân cong tự nhiên.', 'Đầu tôm còn bám chắc vào thân.', 'Mùi biển nhẹ, không hôi.'],
    avoidSigns: ['Tránh tôm đầu rời, thân mềm nhũn.', 'Không chọn tôm có mùi khai hoặc hôi mạnh.', 'Tránh tôm đen đầu nhiều, nhớt bất thường.'],
    storage: ['Giữ lạnh và dùng càng sớm càng tốt.', 'Cấp đông nếu chưa dùng ngay.', 'Không để tôm sống chạm rau ăn liền.'],
    safetyNote: 'Tôm cần nấu chín kỹ. Người dị ứng hải sản nên tránh.'
  },
  {
    id: 20,
    nameVi: 'Mực',
    nameEn: 'Squid',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/muc.jpg',
    shortDesc: 'Hải sản dùng cho món xào, hấp, nướng hoặc lẩu.',
    chooseTips: ['Chọn mực thân sáng, da bám tương đối chắc.', 'Thịt mực đàn hồi khi ấn.', 'Mắt trong, không đục quá nhiều.'],
    avoidSigns: ['Tránh mực mùi hôi gắt hoặc khai.', 'Không chọn mực mềm nhũn, nhớt nhiều.', 'Tránh thân mực thâm lạ hoặc chảy dịch.'],
    storage: ['Bảo quản lạnh ngay sau khi mua.', 'Làm sạch trước khi nấu.', 'Cấp đông nếu chưa dùng ngay.'],
    safetyNote: 'Mực cần nấu chín kỹ. Người dị ứng hải sản nên tránh.'
  },
  {
    id: 21,
    nameVi: 'Trứng gà',
    nameEn: 'Chicken Egg',
    category: 'trung-sua',
    categoryLabel: 'Trứng & sữa',
    image: '/assets/images/food-guide/trung-sua/trung-ga.jpg',
    shortDesc: 'Nguyên liệu dễ dùng cho món chiên, luộc, kho hoặc làm bánh.',
    chooseTips: ['Chọn trứng vỏ sạch, không nứt.', 'Cầm chắc tay, không có mùi lạ.', 'Có thể thả vào nước: trứng chìm thường còn tươi hơn.'],
    avoidSigns: ['Tránh trứng nứt, bẩn nhiều, có mùi hôi.', 'Không dùng trứng nổi hẳn trên mặt nước.', 'Tránh trứng chảy dịch hoặc dính bẩn bất thường.'],
    storage: ['Để nơi mát hoặc ngăn mát tủ lạnh.', 'Không rửa trứng trước khi cất lâu.', 'Để trứng tách khỏi thực phẩm có mùi mạnh.'],
    safetyNote: 'Nấu chín trứng để giảm nguy cơ nhiễm khuẩn. Không dùng trứng có mùi lạ.'
  },
  {
    id: 22,
    nameVi: 'Sữa tươi',
    nameEn: 'Fresh Milk',
    category: 'trung-sua',
    categoryLabel: 'Trứng & sữa',
    image: '/assets/images/food-guide/trung-sua/sua-tuoi.jpg',
    shortDesc: 'Thức uống và nguyên liệu làm soup, sinh tố, bánh hoặc sốt.',
    chooseTips: ['Chọn sản phẩm còn hạn sử dụng.', 'Bao bì nguyên vẹn, không phồng rộp.', 'Ưu tiên nơi bán có bảo quản lạnh đúng cách nếu là sữa thanh trùng.'],
    avoidSigns: ['Tránh hộp móp, rò rỉ hoặc phồng.', 'Không dùng sữa có mùi chua, vón cục.', 'Tránh sản phẩm hết hạn.'],
    storage: ['Bảo quản theo hướng dẫn trên bao bì.', 'Sau khi mở nắp, để ngăn mát và dùng sớm.', 'Không để sữa ngoài nhiệt độ phòng quá lâu.'],
    safetyNote: 'Người dị ứng sữa hoặc không dung nạp lactose nên chọn sản phẩm thay thế phù hợp.'
  },
  {
    id: 23,
    nameVi: 'Đậu hũ',
    nameEn: 'Tofu',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/dau-hu.jpg',
    shortDesc: 'Thực phẩm từ đậu nành, dùng chiên, kho, sốt hoặc nấu canh.',
    chooseTips: ['Chọn đậu hũ màu trắng ngà tự nhiên.', 'Bề mặt mềm nhưng không nhớt.', 'Mùi thơm nhẹ của đậu, không chua.'],
    avoidSigns: ['Tránh đậu hũ chảy nước đục, nhớt.', 'Không chọn miếng có mùi chua hoặc mốc.', 'Tránh bao bì rách nếu mua đóng gói.'],
    storage: ['Để ngăn mát và dùng sớm.', 'Ngâm trong nước sạch nếu cần bảo quản ngắn.', 'Thay nước mỗi ngày nếu chưa dùng.'],
    safetyNote: 'Đậu hũ có đậu nành. Không dùng nếu có mùi chua hoặc bề mặt nhớt.'
  },
  {
    id: 24,
    nameVi: 'Đậu nành',
    nameEn: 'Soybean',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/dau-nanh.jpg',
    shortDesc: 'Hạt dùng làm sữa đậu nành, đậu hũ hoặc món hầm.',
    chooseTips: ['Chọn hạt khô, đều màu.', 'Hạt không có mùi mốc.', 'Bao bì sạch, không lẫn côn trùng.'],
    avoidSigns: ['Tránh hạt mốc, đổi màu bất thường.', 'Không chọn hạt có mùi ẩm hoặc chua.', 'Tránh túi có mọt hoặc bụi bẩn nhiều.'],
    storage: ['Để nơi khô ráo, kín.', 'Tránh ánh nắng trực tiếp và nơi ẩm.', 'Sau khi mở bao, buộc kín hoặc cho vào hộp.'],
    safetyNote: 'Sữa đậu nành tự nấu phải được đun sôi kỹ. Người dị ứng đậu nành nên tránh.'
  },
  {
    id: 25,
    nameVi: 'Gạo lứt',
    nameEn: 'Brown Rice',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/gao-lut.jpg',
    shortDesc: 'Ngũ cốc nguyên cám, dùng cho cơm healthy hoặc cháo.',
    chooseTips: ['Chọn hạt khô, không ẩm.', 'Mùi thơm nhẹ tự nhiên của gạo.', 'Bao bì có nguồn gốc và hạn dùng rõ.'],
    avoidSigns: ['Tránh gạo mốc, có mọt.', 'Không dùng gạo có mùi hôi dầu hoặc ẩm.', 'Tránh hạt đổi màu bất thường.'],
    storage: ['Để trong hộp kín, nơi khô mát.', 'Tránh nơi ẩm và ánh nắng trực tiếp.', 'Mua lượng vừa đủ để dùng trong thời gian hợp lý.'],
    safetyNote: 'Vo gạo trước khi nấu. Không dùng gạo có dấu hiệu mốc vì có thể không an toàn.'
  },
  {
    id: 26,
    nameVi: 'Yến mạch',
    nameEn: 'Oats',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/yen-mach.jpg',
    shortDesc: 'Ngũ cốc dùng cho bữa sáng, cháo, bánh hoặc overnight oats.',
    chooseTips: ['Chọn sản phẩm còn hạn sử dụng.', 'Hạt khô, không vón cục do ẩm.', 'Mùi nhẹ tự nhiên, không hôi dầu.'],
    avoidSigns: ['Tránh yến mạch mốc, có côn trùng.', 'Không dùng nếu có mùi hôi dầu.', 'Tránh bao bì rách hoặc ẩm.'],
    storage: ['Đậy kín sau khi mở.', 'Để nơi khô ráo, thoáng mát.', 'Có thể bảo quản trong hộp kín để tránh ẩm.'],
    safetyNote: 'Một số yến mạch có thể lẫn gluten trong quá trình sản xuất; người dị ứng gluten cần kiểm tra nhãn.'
  },
  {
    id: 27,
    nameVi: 'Hành tím',
    nameEn: 'Shallot',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/hanh-tim.jpg',
    shortDesc: 'Gia vị thơm dùng để phi, ướp thịt cá hoặc nấu nước dùng.',
    chooseTips: ['Chọn củ khô, vỏ ngoài lành.', 'Củ chắc tay, không mềm.', 'Mùi hành thơm tự nhiên.'],
    avoidSigns: ['Tránh củ mốc, mọc mầm nhiều.', 'Không chọn củ mềm nhũn hoặc chảy nước.', 'Tránh mùi thối hoặc ẩm mốc.'],
    storage: ['Để nơi khô thoáng.', 'Không để trong túi kín ẩm lâu ngày.', 'Tránh ánh nắng trực tiếp.'],
    safetyNote: 'Bỏ phần mốc hoặc hư. Rửa sạch sau khi bóc nếu dùng ăn sống hoặc trộn gỏi.'
  },
  {
    id: 28,
    nameVi: 'Tỏi',
    nameEn: 'Garlic',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/toi.jpg',
    shortDesc: 'Gia vị thơm dùng trong món xào, nướng, nước chấm và ướp.',
    chooseTips: ['Chọn củ chắc, tép đầy.', 'Vỏ khô, không ẩm mốc.', 'Mùi tỏi thơm rõ nhưng không hôi thối.'],
    avoidSigns: ['Tránh tỏi mốc, mềm hoặc chảy nước.', 'Không chọn củ mọc mầm quá nhiều nếu cần vị thơm nhẹ.', 'Tránh tép có đốm thối đen.'],
    storage: ['Để nơi khô, thoáng.', 'Không bảo quản tỏi khô trong tủ lạnh ẩm.', 'Tỏi bóc vỏ nên để hộp kín trong ngăn mát và dùng sớm.'],
    safetyNote: 'Không dùng tỏi ngâm dầu để lâu ở nhiệt độ phòng vì có nguy cơ không an toàn.'
  },
  {
    id: 29,
    nameVi: 'Gừng',
    nameEn: 'Ginger',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/gung.jpg',
    shortDesc: 'Gia vị ấm, dùng khử mùi thịt cá, kho gà hoặc pha trà.',
    chooseTips: ['Chọn củ chắc, vỏ mỏng vừa.', 'Mùi gừng thơm cay tự nhiên.', 'Củ không bị mềm hoặc mốc.'],
    avoidSigns: ['Tránh gừng nhũn, mốc xanh hoặc đen.', 'Không chọn củ chảy nước.', 'Tránh mùi lạ hoặc quá khô teo.'],
    storage: ['Để nơi khô thoáng nếu dùng nhanh.', 'Có thể bọc kín để ngăn mát.', 'Gừng đã cắt nên dùng sớm.'],
    safetyNote: 'Rửa sạch đất bám trước khi cắt. Bỏ phần mốc thay vì chỉ gọt sơ nếu mốc lan rộng.'
  },
  {
    id: 30,
    nameVi: 'Nấm khô',
    nameEn: 'Dried Mushroom',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/nam-kho.jpg',
    shortDesc: 'Thực phẩm khô tạo vị ngọt cho canh, lẩu, món chay và món hầm.',
    chooseTips: ['Chọn nấm khô đều màu, không vụn quá nhiều.', 'Mùi thơm đặc trưng, không mốc.', 'Bao bì kín, có nguồn gốc rõ ràng.'],
    avoidSigns: ['Tránh nấm có mùi ẩm mốc.', 'Không chọn nấm có đốm mốc trắng/xanh bất thường.', 'Tránh túi nấm có mọt hoặc ẩm.'],
    storage: ['Để hộp kín nơi khô ráo.', 'Tránh nơi ẩm và ánh nắng trực tiếp.', 'Sau khi ngâm nấm, dùng trong ngày.'],
    safetyNote: 'Ngâm rửa kỹ trước khi nấu. Không dùng nấm khô bị mốc vì có thể không an toàn.'
  }
];

const FOOD_GUIDE_DETAILED_ITEMS = [
  {
    id: 1,
    nameVi: 'Rau cải xanh',
    nameEn: 'Mustard Greens',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/rau-cai-xanh.jpg',
    shortDesc: 'Rau lá xanh dùng để luộc, xào tỏi, nấu canh hoặc ăn kèm bữa cơm gia đình.',
    chooseTips: ['Lá xanh tự nhiên, không bóng bất thường và không héo rũ.', 'Cọng rau giòn, bẻ nhẹ nghe tiếng gãy, không mềm nhũn.', 'Gốc rau còn tươi, không bị úng nước hoặc nhớt.', 'Bó rau có mùi rau tươi nhẹ, không nồng mùi hóa chất.'],
    avoidSigns: ['Lá vàng úa nhiều, thâm đen hoặc dập nát lan rộng.', 'Cọng rau bị nhớt, chảy nước hoặc có mùi chua.', 'Rau có đốm mốc trắng/xám ở gốc hoặc mặt lá.', 'Bó rau quá non, xanh đậm bất thường và đồng đều một cách lạ thường.'],
    storage: ['Nhặt bỏ lá hư trước khi cất.', 'Không rửa trước nếu chưa nấu ngay để rau ít úng.', 'Bọc bằng giấy bếp hoặc túi thoáng rồi để ngăn mát.', 'Dùng tốt nhất trong 1-2 ngày sau khi mua.'],
    safetyNote: 'Rửa từng bẹ lá dưới vòi nước, ngâm nước sạch vài phút rồi rửa lại. Nấu chín nếu dùng cho trẻ nhỏ, người lớn tuổi hoặc người có tiêu hóa nhạy cảm.'
  },
  {
    id: 2,
    nameVi: 'Rau muống',
    nameEn: 'Water Spinach',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/rau-muong.jpg',
    shortDesc: 'Rau phổ biến cho món luộc, xào tỏi, nấu canh chua hoặc ăn kèm món kho.',
    chooseTips: ['Cọng rau vừa phải, không quá to già hoặc quá mềm.', 'Lá xanh, phần ngọn còn tươi và không dập.', 'Thân rau giòn, mặt cắt không thâm đen.', 'Rau không có mùi bùn, mùi chua hoặc mùi lạ.'],
    avoidSigns: ['Cọng rau rỗng quá to, già xơ và lá úa nhiều.', 'Rau bị nhớt ở thân hoặc phần gốc.', 'Có vết sâu hỏng lan rộng trên lá và ngọn.', 'Rau héo rũ, thâm đen hoặc chảy dịch.'],
    storage: ['Giữ rau khô ráo, bọc lỏng bằng túi có lỗ thoáng.', 'Đặt ở ngăn mát, tránh ép dưới đồ nặng.', 'Không để sát thịt cá sống chưa bọc kín.', 'Nên dùng trong ngày hoặc tối đa 1-2 ngày.'],
    safetyNote: 'Rửa kỹ phần cọng vì dễ bám bùn. Nếu không rõ nguồn gốc, nên nấu chín hoàn toàn thay vì ăn tái.'
  },
  {
    id: 3,
    nameVi: 'Cà chua',
    nameEn: 'Tomato',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/ca-chua.jpg',
    shortDesc: 'Quả dùng cho canh, sốt, salad, món xào và nhiều món ăn gia đình.',
    chooseTips: ['Vỏ căng, màu đỏ hoặc cam đỏ tự nhiên tùy độ chín.', 'Quả cầm chắc tay, không quá mềm ở phần cuống.', 'Cuống còn xanh nhẹ hoặc khô sạch, không mốc.', 'Mùi cà chua tươi nhẹ, không có mùi lên men.'],
    avoidSigns: ['Quả nứt vỏ, dập, chảy nước hoặc mềm nhũn.', 'Có đốm mốc ở cuống hoặc vết thâm lan rộng.', 'Mùi chua gắt, mùi rượu hoặc mùi hôi.', 'Vỏ nhăn nhiều, quả quá nhẹ và khô ruột.'],
    storage: ['Cà chua còn cứng để nơi thoáng mát cho chín tự nhiên.', 'Cà chua chín mềm nên để ngăn mát và dùng sớm.', 'Không rửa trước khi cất nếu chưa dùng ngay.', 'Phần đã cắt phải bọc kín và dùng trong ngày.'],
    safetyNote: 'Rửa sạch vỏ trước khi cắt. Không dùng phần quả bị mốc vì nấm mốc có thể lan sâu vào phần thịt mềm.'
  },
  {
    id: 4,
    nameVi: 'Cà rốt',
    nameEn: 'Carrot',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/ca-rot.jpg',
    shortDesc: 'Củ giòn ngọt dùng cho canh, xào, salad, nước ép và món hầm.',
    chooseTips: ['Củ màu cam đều, cầm chắc và nặng tay.', 'Bề mặt tương đối mịn, ít nứt sâu.', 'Đầu củ không bị mềm, không thâm đen.', 'Nếu còn lá, lá không héo úa quá nhiều.'],
    avoidSigns: ['Củ mềm nhũn, cong quẹo nhiều và teo khô.', 'Có mốc, chảy nước hoặc mùi lạ.', 'Vết nứt sâu bị thâm đen vào trong.', 'Đầu củ úng hoặc có lớp nhớt.'],
    storage: ['Cắt bỏ phần lá nếu còn để củ lâu héo hơn.', 'Lau khô, bọc kín vừa phải rồi để ngăn mát.', 'Không để gần trái cây chín mạnh như táo, chuối trong thời gian dài.', 'Cà rốt đã gọt/cắt nên cho hộp kín và dùng sớm.'],
    safetyNote: 'Rửa và gọt vỏ nếu ăn sống hoặc ép nước. Bỏ phần bị mốc, úng sâu thay vì chỉ cắt rất mỏng bên ngoài.'
  },
  {
    id: 5,
    nameVi: 'Khoai tây',
    nameEn: 'Potato',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/khoai-tay.jpg',
    shortDesc: 'Củ giàu tinh bột dùng để chiên, hầm, nghiền, nấu soup hoặc làm món nướng.',
    chooseTips: ['Củ chắc, vỏ khô, không ẩm nhớt.', 'Bề mặt ít vết thâm sâu và không có mảng xanh.', 'Mầm chưa mọc hoặc chỉ rất ít mầm nhỏ.', 'Củ có kích thước tương đối đồng đều nếu nấu cùng mẻ.'],
    avoidSigns: ['Khoai có vỏ xanh, mọc mầm nhiều hoặc mềm nhũn.', 'Có mùi mốc, mùi đất ẩm hôi hoặc chảy nước.', 'Vết thối đen lan sâu vào ruột.', 'Củ quá nhăn, teo và nhẹ bất thường.'],
    storage: ['Để nơi khô, tối, thoáng khí.', 'Không cất trong tủ lạnh vì dễ đổi vị và ảnh hưởng kết cấu.', 'Không rửa trước khi bảo quản.', 'Tách khỏi hành tây để hạn chế mọc mầm nhanh.'],
    safetyNote: 'Không ăn khoai tây xanh hoặc mọc mầm nhiều. Nếu chỉ có mầm nhỏ, gọt bỏ sâu phần mầm và vùng xanh trước khi nấu.'
  },
  {
    id: 6,
    nameVi: 'Bí đỏ',
    nameEn: 'Pumpkin',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/bi-do.jpg',
    shortDesc: 'Bí vị ngọt bùi dùng nấu canh, soup, cháo, hấp hoặc làm món healthy.',
    chooseTips: ['Vỏ cứng, màu vỏ và thịt bí đều tự nhiên.', 'Miếng bí cắt sẵn có thịt chắc, không nhũn.', 'Ruột bí không mốc, không có mùi chua.', 'Cầm miếng bí thấy nặng tay so với kích thước.'],
    avoidSigns: ['Bí chảy nước, nhớt hoặc có mùi lên men.', 'Mặt cắt khô đen quá nhiều hoặc thối mềm.', 'Ruột bí mốc trắng/xanh.', 'Vỏ có vùng thối lan rộng.'],
    storage: ['Bí nguyên quả để nơi thoáng, khô, tránh nắng trực tiếp.', 'Bí đã cắt bọc kín mặt cắt rồi để ngăn mát.', 'Dùng phần đã cắt trong vài ngày.', 'Không để bí cắt sẵn gần thực phẩm sống có mùi mạnh.'],
    safetyNote: 'Rửa sạch vỏ trước khi bổ để bụi bẩn không dính vào dao và phần thịt bí.'
  },
  {
    id: 7,
    nameVi: 'Cam',
    nameEn: 'Orange',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/cam.jpg',
    shortDesc: 'Trái cây mọng nước dùng ăn trực tiếp, ép nước hoặc làm món tráng miệng.',
    chooseTips: ['Quả cầm nặng tay, vỏ căng và không quá khô.', 'Vỏ có mùi thơm nhẹ tự nhiên.', 'Đáy quả không mềm nhũn hoặc mốc.', 'Kích thước vừa phải, chắc tay thường nhiều nước hơn.'],
    avoidSigns: ['Quả mềm nhũn, chảy nước hoặc có mốc ở vỏ.', 'Mùi lên men, mùi rượu hoặc mùi chua gắt.', 'Vỏ thâm lan rộng, lõm sâu.', 'Quả quá nhẹ, vỏ khô teo.'],
    storage: ['Để nơi thoáng nếu dùng trong 1-2 ngày.', 'Muốn giữ lâu hơn thì để ngăn mát.', 'Cam đã cắt hoặc đã vắt nên dùng sớm.', 'Không để cam hỏng chung với cam lành vì dễ lây mốc.'],
    safetyNote: 'Rửa vỏ trước khi cắt hoặc vắt để hạn chế bụi bẩn đi vào phần nước cam.'
  },
  {
    id: 8,
    nameVi: 'Táo',
    nameEn: 'Apple',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/tao.jpg',
    shortDesc: 'Trái cây giòn ngọt, tiện làm bữa phụ, salad hoặc nước ép.',
    chooseTips: ['Quả chắc tay, vỏ căng và không bị nhăn nhiều.', 'Màu sắc đều theo giống táo, không thâm lõm.', 'Cuống khô sạch, không mốc.', 'Ấn nhẹ không thấy vùng mềm bầm lớn.'],
    avoidSigns: ['Quả bầm mềm, nứt sâu hoặc có mùi lạ.', 'Mốc quanh cuống hoặc đáy quả.', 'Vỏ nhăn, quả nhẹ và xốp.', 'Vùng thối nâu lan rộng vào thịt quả.'],
    storage: ['Để ngăn mát để giữ độ giòn lâu hơn.', 'Tách khỏi quả đã hỏng hoặc quá chín.', 'Táo đã cắt nên bọc kín, có thể thêm ít nước chanh để hạn chế thâm.', 'Rửa ngay trước khi ăn, không rửa rồi cất lâu.'],
    safetyNote: 'Nếu ăn cả vỏ, rửa kỹ dưới vòi nước và chà nhẹ bề mặt. Bỏ phần bầm sâu hoặc mốc.'
  },
  {
    id: 9,
    nameVi: 'Chuối',
    nameEn: 'Banana',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/chuoi.jpg',
    shortDesc: 'Trái cây mềm ngọt dùng ăn nhanh, làm sinh tố, bánh hoặc bữa phụ.',
    chooseTips: ['Chọn độ chín theo nhu cầu: xanh để để lâu, vàng để ăn ngay.', 'Vỏ vàng đều, ít đốm nâu là chuối chín ngọt.', 'Quả còn nguyên, không bị dập nát lớn.', 'Cuống khô sạch, không mốc.'],
    avoidSigns: ['Cuống mốc, quả chảy nước hoặc có mùi lên men.', 'Vỏ đen mềm nhũn toàn bộ.', 'Quả nứt to để lộ phần thịt lâu ngoài không khí.', 'Nải chuối bị dập nát nhiều quả liền nhau.'],
    storage: ['Để nhiệt độ phòng cho chuối chín tự nhiên.', 'Tách từng quả nếu muốn làm chậm chín lan cả nải.', 'Chuối chín quá có thể bóc vỏ, cấp đông để xay sinh tố.', 'Không để chuối sát thực phẩm có mùi mạnh.'],
    safetyNote: 'Không dùng phần thịt chuối có mốc hoặc mùi chua. Rửa tay sau khi bóc nếu vỏ bẩn.'
  },
  {
    id: 10,
    nameVi: 'Bơ',
    nameEn: 'Avocado',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/bo.jpg',
    shortDesc: 'Trái béo nhẹ dùng làm sinh tố, salad, bánh mì hoặc món ăn lành mạnh.',
    chooseTips: ['Quả cầm chắc, hơi mềm đều khi đã chín.', 'Vỏ không nứt sâu, không mốc ở cuống.', 'Cuống bật nhẹ thấy màu vàng xanh thường vừa chín.', 'Không có mùi chua hoặc mùi lên men.'],
    avoidSigns: ['Quả mềm nhũn, chảy nước hoặc bầm đen nhiều.', 'Mốc quanh cuống hoặc vết nứt.', 'Thịt bơ xơ đen, đắng hoặc có mùi lạ.', 'Quả quá cứng nếu cần dùng ngay trong ngày.'],
    storage: ['Bơ chưa chín để nhiệt độ phòng.', 'Bơ chín để ngăn mát và dùng trong 1-2 ngày.', 'Bơ đã cắt bọc sát mặt, thêm ít chanh để giảm thâm.', 'Không để bơ đã cắt hở lâu ngoài nhiệt độ phòng.'],
    safetyNote: 'Rửa vỏ trước khi cắt vì dao có thể kéo bụi bẩn từ vỏ vào phần thịt.'
  },
  {
    id: 11,
    nameVi: 'Dưa hấu',
    nameEn: 'Watermelon',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/dua-hau.jpg',
    shortDesc: 'Trái nhiều nước dùng tráng miệng, ép nước hoặc làm món giải nhiệt.',
    chooseTips: ['Quả chắc, vỏ lành và không nứt.', 'Vết tiếp đất màu vàng kem thường cho thấy quả chín tự nhiên.', 'Gõ nghe âm trầm vừa, không quá rỗng.', 'Cuống khô vừa, không mốc hoặc chảy nước.'],
    avoidSigns: ['Quả nứt, mềm bất thường hoặc rỉ nước.', 'Mùi chua, mùi lên men ở vỏ hoặc phần cắt.', 'Miếng cắt sẵn để ngoài lâu, không che chắn.', 'Ruột mềm nhũn, nhớt hoặc có bọt lạ.'],
    storage: ['Dưa nguyên quả để nơi thoáng nếu chưa bổ.', 'Dưa đã cắt phải bọc kín và để ngăn mát.', 'Dùng dưa đã cắt trong thời gian ngắn.', 'Dùng dao và thớt sạch riêng cho trái cây ăn trực tiếp.'],
    safetyNote: 'Rửa sạch vỏ trước khi bổ vì vi khuẩn trên vỏ có thể đi theo dao vào phần ruột.'
  },
  {
    id: 12,
    nameVi: 'Xoài',
    nameEn: 'Mango',
    category: 'trai-cay',
    categoryLabel: 'Trái cây',
    image: '/assets/images/food-guide/trai-cay/xoai.jpg',
    shortDesc: 'Trái cây thơm ngọt hoặc chua nhẹ, dùng ăn trực tiếp, làm gỏi, sinh tố.',
    chooseTips: ['Quả có mùi thơm nhẹ ở cuống khi gần chín.', 'Cầm chắc tay, độ mềm phù hợp món cần dùng.', 'Vỏ không có vết thối sâu hoặc mốc.', 'Quả xanh dùng làm gỏi nên cứng đều, không nhũn một bên.'],
    avoidSigns: ['Quả chảy nhựa nhiều, mềm nhũn hoặc mùi lên men.', 'Mảng thâm đen lan rộng vào thịt quả.', 'Vỏ nứt sâu có côn trùng hoặc mốc.', 'Thịt quả có vị lạ, nhớt hoặc đắng bất thường.'],
    storage: ['Xoài xanh hoặc chưa chín để nhiệt độ phòng.', 'Xoài chín để ngăn mát và dùng sớm.', 'Xoài đã cắt cho hộp kín, tránh để hở.', 'Không để xoài chín sát thực phẩm có mùi mạnh.'],
    safetyNote: 'Rửa vỏ trước khi gọt. Không dùng phần thịt bị mốc hoặc lên men.'
  },
  {
    id: 13,
    nameVi: 'Thịt heo',
    nameEn: 'Pork',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-heo.jpg',
    shortDesc: 'Nguồn đạm phổ biến cho món kho, luộc, xào, nướng và nấu canh.',
    chooseTips: ['Màu hồng tươi hoặc đỏ nhạt tự nhiên, không tái xanh.', 'Bề mặt hơi khô, không nhớt và không chảy dịch nhiều.', 'Ấn nhẹ có độ đàn hồi, vết lõm phục hồi nhanh.', 'Mỡ trắng ngà, không vàng sậm hoặc có mùi hôi.'],
    avoidSigns: ['Thịt xám, xanh, thâm đen hoặc có đốm lạ.', 'Mùi hôi, mùi chua hoặc mùi ôi dầu.', 'Bề mặt nhớt dính tay, chảy dịch đục.', 'Miếng thịt quá mềm nhũn hoặc bao bì phồng/rò rỉ.'],
    storage: ['Dùng trong ngày thì để ngăn mát trong hộp kín.', 'Chưa dùng ngay nên chia phần và cấp đông.', 'Rã đông trong ngăn mát, không để ngoài quá lâu.', 'Để thịt sống tách riêng rau quả và đồ ăn chín.'],
    safetyNote: 'Nấu chín kỹ thịt heo. Dùng dao thớt riêng cho thịt sống và rửa tay sau khi sơ chế.'
  },
  {
    id: 14,
    nameVi: 'Thịt bò',
    nameEn: 'Beef',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-bo.jpg',
    shortDesc: 'Thịt đỏ dùng cho phở, bún, món xào, hầm, áp chảo hoặc nướng.',
    chooseTips: ['Màu đỏ tươi tự nhiên, thớ thịt rõ.', 'Mỡ bò vàng nhạt, không xám hoặc nhớt.', 'Bề mặt khô ráo, đàn hồi khi ấn.', 'Mùi thịt tươi nhẹ, không hăng chua.'],
    avoidSigns: ['Thịt thâm đen, tái xám hoặc chuyển xanh.', 'Mùi chua, mùi hôi hoặc mùi ôi.', 'Chảy dịch nhiều, bề mặt nhớt.', 'Thịt rã đông lại nhiều lần, mềm bở và nước đỏ đục nhiều.'],
    storage: ['Bảo quản lạnh ngay sau khi mua.', 'Chia phần mỏng trước khi cấp đông để dễ rã đông.', 'Rã đông trong ngăn mát hoặc bằng chế độ phù hợp.', 'Không tái cấp đông thịt đã rã đông hoàn toàn nhiều lần.'],
    safetyNote: 'Người nhạy cảm, trẻ nhỏ hoặc người lớn tuổi nên dùng thịt bò được nấu chín kỹ.'
  },
  {
    id: 15,
    nameVi: 'Thịt gà',
    nameEn: 'Chicken',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/thit-ga.jpg',
    shortDesc: 'Nguồn đạm dễ chế biến cho món luộc, kho, chiên, nướng, salad chín.',
    chooseTips: ['Da gà màu tự nhiên, không bầm tím nhiều.', 'Thịt săn, ấn nhẹ có đàn hồi.', 'Mùi tươi nhẹ, không hôi hoặc tanh gắt.', 'Nếu đóng gói, bao bì còn nguyên và còn hạn dùng.'],
    avoidSigns: ['Gà nhớt, chảy dịch đục hoặc có mùi hôi.', 'Thịt tái xám, thâm đen hoặc bầm bất thường.', 'Bao bì phồng, rò rỉ hoặc quá hạn.', 'Phần da có đốm mốc hoặc nhớt dày.'],
    storage: ['Để ngăn mát nếu nấu trong ngày.', 'Cấp đông nếu chưa dùng ngay.', 'Đựng trong hộp kín để nước thịt không chảy sang thực phẩm khác.', 'Rã đông trong ngăn mát và nấu ngay sau khi rã.'],
    safetyNote: 'Thịt gà phải nấu chín kỹ, không còn màu hồng ở phần dày hoặc gần xương.'
  },
  {
    id: 16,
    nameVi: 'Sườn heo',
    nameEn: 'Pork Ribs',
    category: 'thit',
    categoryLabel: 'Thịt',
    image: '/assets/images/food-guide/thit/suon-heo.jpg',
    shortDesc: 'Phần thịt có xương dùng nấu canh, rim, kho, nướng hoặc hầm.',
    chooseTips: ['Thịt bám xương có màu hồng nhạt tự nhiên.', 'Xương không thâm đen, mặt cắt sạch.', 'Miếng sườn khô ráo, không nhớt.', 'Tỷ lệ nạc mỡ phù hợp món cần nấu.'],
    avoidSigns: ['Sườn có mùi hôi, mùi chua hoặc mùi ôi.', 'Thịt xám xanh, bầm tím bất thường.', 'Chảy dịch nhiều hoặc bề mặt dính nhớt.', 'Xương thâm đen, khô cứng và thịt teo.'],
    storage: ['Bảo quản lạnh ngay sau khi mua.', 'Chia phần theo bữa trước khi cấp đông.', 'Không để sườn sống gần rau quả ăn liền.', 'Rã đông trong ngăn mát rồi nấu chín kỹ.'],
    safetyNote: 'Nấu chín kỹ phần thịt sát xương vì đây là vùng dễ còn sống nếu nấu vội.'
  },
  {
    id: 17,
    nameVi: 'Cá hồi',
    nameEn: 'Salmon',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/ca-hoi.jpg',
    shortDesc: 'Cá béo dùng áp chảo, nướng, làm bowl, salad chín hoặc món healthy.',
    chooseTips: ['Thịt cá màu cam hồng tự nhiên, vân mỡ rõ.', 'Miếng cá săn, ấn nhẹ không lõm sâu.', 'Bề mặt ẩm nhưng không nhớt, không chảy dịch đục.', 'Mùi biển nhẹ, không tanh gắt.'],
    avoidSigns: ['Thịt cá xỉn màu, nâu xám hoặc rời thớ.', 'Mùi khai, tanh nồng hoặc mùi chua.', 'Bề mặt nhớt dày, chảy nước nhiều.', 'Cá rã đông nhiều lần, mềm bở và mất độ đàn hồi.'],
    storage: ['Giữ lạnh liên tục và dùng càng sớm càng tốt.', 'Nếu chưa dùng ngay, cấp đông trong túi kín.', 'Rã đông trong ngăn mát, không ngâm ngoài lâu.', 'Để riêng cá sống khỏi rau ăn liền.'],
    safetyNote: 'Nếu không dùng loại đạt chuẩn ăn sống, nên nấu chín. Người dị ứng hải sản/cá cần tránh.'
  },
  {
    id: 18,
    nameVi: 'Cá basa',
    nameEn: 'Basa Fish',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/ca-basa.jpg',
    shortDesc: 'Cá thịt mềm, vị nhẹ, dùng kho, chiên, nấu canh chua hoặc hấp.',
    chooseTips: ['Thịt cá trắng hồng nhẹ, không xám đục.', 'Miếng cá còn đàn hồi, không bở nát.', 'Mùi tanh nhẹ tự nhiên, không hôi.', 'Nếu đông lạnh, bao bì kín và không đóng tuyết quá dày.'],
    avoidSigns: ['Cá có mùi hôi, khai hoặc chua.', 'Thịt mềm nhũn, rã nước nhiều.', 'Màu xám đen hoặc vàng bất thường.', 'Bao bì rách, có dấu hiệu rã đông rồi đông lại.'],
    storage: ['Để lạnh nếu nấu trong ngày.', 'Cấp đông trong bao kín nếu chưa dùng.', 'Rã đông trong ngăn mát để hạn chế bở thịt.', 'Không để nước cá sống chảy sang thực phẩm khác.'],
    safetyNote: 'Nấu cá chín hoàn toàn, đặc biệt khi dùng cho trẻ nhỏ hoặc người lớn tuổi.'
  },
  {
    id: 19,
    nameVi: 'Tôm',
    nameEn: 'Shrimp',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/tom.jpg',
    shortDesc: 'Hải sản phổ biến dùng hấp, luộc, xào, nấu canh, lẩu hoặc gỏi chín.',
    chooseTips: ['Vỏ tôm trong, thân cong tự nhiên và chắc.', 'Đầu còn bám vào thân, không lỏng rời nhiều.', 'Mùi biển nhẹ, không khai hoặc hôi.', 'Tôm còn độ đàn hồi, không mềm bở.'],
    avoidSigns: ['Đầu tôm đen nhiều, rời khỏi thân hoặc chảy dịch.', 'Mùi khai, hôi nồng hoặc chua.', 'Thân mềm nhũn, vỏ nhớt.', 'Tôm đông lạnh đóng tuyết dày, có dấu hiệu rã đông lại.'],
    storage: ['Giữ lạnh và dùng trong ngày để ngon nhất.', 'Cấp đông nếu chưa nấu ngay.', 'Bọc kín để không ám mùi sang thực phẩm khác.', 'Rã đông trong ngăn mát rồi chế biến sớm.'],
    safetyNote: 'Tôm cần nấu chín kỹ. Người dị ứng hải sản nên tránh hoàn toàn.'
  },
  {
    id: 20,
    nameVi: 'Mực',
    nameEn: 'Squid',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/muc.jpg',
    shortDesc: 'Hải sản có độ giòn, dùng xào, hấp, nướng, chiên hoặc nấu lẩu.',
    chooseTips: ['Thân mực sáng, da bám tương đối chắc.', 'Thịt đàn hồi khi ấn, không mềm nhũn.', 'Mắt trong vừa, không đục quá nhiều.', 'Mùi biển nhẹ, không khai gắt.'],
    avoidSigns: ['Mực có mùi hôi, khai hoặc chua.', 'Thân mực mềm nhũn, nhớt nhiều.', 'Da bong tróc bất thường, thịt thâm xám.', 'Chảy dịch đục hoặc bị rã đông nhiều lần.'],
    storage: ['Làm sạch và chế biến sớm sau khi mua.', 'Nếu chưa dùng, cấp đông trong túi kín.', 'Rã đông trong ngăn mát.', 'Không để mực sống tiếp xúc rau ăn liền.'],
    safetyNote: 'Nấu chín mực trước khi ăn. Người dị ứng hải sản nên tránh.'
  },
  {
    id: 21,
    nameVi: 'Trứng gà',
    nameEn: 'Chicken Egg',
    category: 'trung-sua',
    categoryLabel: 'Trứng & sữa',
    image: '/assets/images/food-guide/trung-sua/trung-ga.jpg',
    shortDesc: 'Nguyên liệu nhanh gọn cho món chiên, luộc, kho, canh và làm bánh.',
    chooseTips: ['Vỏ trứng sạch, nguyên vẹn, không nứt.', 'Cầm chắc tay, không có mùi hôi.', 'Trứng chìm trong nước thường còn tươi hơn trứng nổi.', 'Mua trứng có ngày đóng gói hoặc nguồn rõ ràng nếu có thể.'],
    avoidSigns: ['Trứng nứt, dính bẩn nhiều hoặc chảy dịch.', 'Trứng có mùi hôi khi đập ra.', 'Lòng trắng quá loãng, lòng đỏ vỡ và mùi lạ.', 'Trứng nổi hẳn trên mặt nước khi kiểm tra.'],
    storage: ['Để nơi mát hoặc ngăn mát tủ lạnh.', 'Không rửa trứng trước khi cất lâu vì dễ mất lớp bảo vệ vỏ.', 'Đặt đầu nhọn xuống nếu khay cho phép.', 'Tránh để gần thực phẩm mùi mạnh.'],
    safetyNote: 'Nấu chín trứng để giảm nguy cơ nhiễm khuẩn, nhất là cho trẻ nhỏ, người lớn tuổi và phụ nữ mang thai.'
  },
  {
    id: 22,
    nameVi: 'Sữa tươi',
    nameEn: 'Fresh Milk',
    category: 'trung-sua',
    categoryLabel: 'Trứng & sữa',
    image: '/assets/images/food-guide/trung-sua/sua-tuoi.jpg',
    shortDesc: 'Thức uống và nguyên liệu cho sinh tố, soup, bánh, sốt hoặc món tráng miệng.',
    chooseTips: ['Chọn sản phẩm còn hạn sử dụng rõ ràng.', 'Bao bì nguyên vẹn, không phồng, móp nặng hoặc rò rỉ.', 'Sữa thanh trùng cần được bảo quản lạnh đúng cách.', 'Ưu tiên sản phẩm có nhãn thành phần và nguồn gốc rõ.'],
    avoidSigns: ['Hộp phồng, rò rỉ, móp méo bất thường.', 'Sữa có mùi chua, vón cục hoặc tách lớp lạ.', 'Sản phẩm hết hạn hoặc bị để ngoài nhiệt độ phòng quá lâu.', 'Nắp chai/hộp bị hở trước khi mua.'],
    storage: ['Bảo quản theo hướng dẫn trên bao bì.', 'Sau khi mở nắp, để ngăn mát và dùng sớm.', 'Không uống trực tiếp từ hộp lớn nếu muốn bảo quản tiếp.', 'Không để sữa ngoài nhiệt độ phòng quá lâu.'],
    safetyNote: 'Người dị ứng sữa hoặc không dung nạp lactose nên chọn sản phẩm thay thế phù hợp.'
  },
  {
    id: 23,
    nameVi: 'Đậu hũ',
    nameEn: 'Tofu',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/dau-hu.jpg',
    shortDesc: 'Thực phẩm từ đậu nành dùng chiên, kho, sốt cà, nấu canh hoặc món chay.',
    chooseTips: ['Miếng đậu màu trắng ngà tự nhiên, không vàng sậm.', 'Bề mặt mềm nhưng không nhớt.', 'Mùi thơm nhẹ của đậu, không chua.', 'Nước ngâm trong tương đối sạch nếu mua dạng ngâm.'],
    avoidSigns: ['Đậu hũ chảy nước đục, nhớt hoặc có mùi chua.', 'Bề mặt mốc, đổi màu hoặc rỗ bất thường.', 'Bao bì rách, phồng hoặc hết hạn.', 'Miếng đậu quá bở nát khi chưa chế biến.'],
    storage: ['Để ngăn mát và dùng sớm.', 'Nếu bảo quản ngắn, ngâm trong nước sạch và thay nước mỗi ngày.', 'Đậy kín để tránh ám mùi tủ lạnh.', 'Không để đậu hũ ở nhiệt độ phòng lâu.'],
    safetyNote: 'Đậu hũ có đậu nành. Người dị ứng đậu nành nên tránh.'
  },
  {
    id: 24,
    nameVi: 'Đậu nành',
    nameEn: 'Soybean',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/dau-nanh.jpg',
    shortDesc: 'Hạt dùng nấu sữa đậu nành, làm đậu hũ, món hầm hoặc món chay.',
    chooseTips: ['Hạt khô, đều màu, ít hạt vỡ vụn.', 'Không có mùi ẩm mốc hoặc mùi chua.', 'Bao bì sạch, kín, không lẫn mọt.', 'Hạt không bị đổi màu đen/xanh bất thường.'],
    avoidSigns: ['Hạt mốc, ẩm, vón cục hoặc có côn trùng.', 'Mùi hôi dầu, chua hoặc mùi kho ẩm.', 'Túi bị rách, bụi bẩn nhiều.', 'Hạt teo lép quá nhiều.'],
    storage: ['Để trong hộp kín nơi khô ráo.', 'Tránh ánh nắng trực tiếp và nơi ẩm.', 'Sau khi mở bao, buộc kín hoặc sang hộp.', 'Không để gần gia vị mùi mạnh.'],
    safetyNote: 'Sữa đậu nành tự nấu phải được đun sôi kỹ. Người dị ứng đậu nành nên tránh.'
  },
  {
    id: 25,
    nameVi: 'Gạo lứt',
    nameEn: 'Brown Rice',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/gao-lut.jpg',
    shortDesc: 'Ngũ cốc nguyên cám dùng nấu cơm healthy, cháo, cơm cuộn hoặc bowl.',
    chooseTips: ['Hạt khô, rời, không ẩm dính.', 'Mùi thơm nhẹ tự nhiên, không hôi dầu.', 'Bao bì có hạn dùng và nguồn gốc rõ.', 'Hạt tương đối đều, ít bụi và tạp chất.'],
    avoidSigns: ['Gạo có mọt, mốc hoặc vón cục.', 'Mùi hôi dầu, mùi ẩm hoặc chua.', 'Hạt đổi màu bất thường, có đốm mốc.', 'Bao bì rách hoặc bảo quản nơi ẩm.'],
    storage: ['Để trong hộp kín, nơi khô mát.', 'Tránh ánh nắng trực tiếp và nền nhà ẩm.', 'Mua lượng vừa đủ vì gạo lứt dễ hôi dầu hơn gạo trắng.', 'Đậy kín sau mỗi lần lấy.'],
    safetyNote: 'Vo/rửa gạo trước khi nấu. Không dùng gạo có dấu hiệu mốc vì có thể không an toàn.'
  },
  {
    id: 26,
    nameVi: 'Yến mạch',
    nameEn: 'Oats',
    category: 'dau-hat-ngu-coc',
    categoryLabel: 'Đậu, hạt & ngũ cốc',
    image: '/assets/images/food-guide/dau-hat-ngu-coc/yen-mach.jpg',
    shortDesc: 'Ngũ cốc dùng cho bữa sáng, cháo, bánh, overnight oats hoặc sinh tố.',
    chooseTips: ['Sản phẩm còn hạn dùng và bao bì kín.', 'Hạt/miếng yến mạch khô, không vón cục do ẩm.', 'Mùi ngũ cốc nhẹ, không hôi dầu.', 'Không thấy mọt hoặc bột mốc trong túi.'],
    avoidSigns: ['Yến mạch mốc, có côn trùng hoặc vón ẩm.', 'Mùi hôi dầu, đắng hoặc chua.', 'Bao bì rách, để nơi ẩm.', 'Màu sắc đổi lạ so với sản phẩm bình thường.'],
    storage: ['Đậy kín sau khi mở.', 'Để nơi khô ráo, thoáng mát.', 'Có thể sang hộp kín để tránh ẩm và mọt.', 'Không dùng muỗng ướt lấy yến mạch.'],
    safetyNote: 'Một số sản phẩm có thể lẫn gluten trong sản xuất; người dị ứng gluten cần kiểm tra nhãn.'
  },
  {
    id: 27,
    nameVi: 'Hành tím',
    nameEn: 'Shallot',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/hanh-tim.jpg',
    shortDesc: 'Gia vị tạo mùi thơm cho món kho, xào, canh, nước dùng và nước chấm.',
    chooseTips: ['Củ chắc tay, vỏ ngoài khô và lành.', 'Mùi hành thơm tự nhiên, không hôi ẩm.', 'Không mọc mầm nhiều nếu cần bảo quản lâu.', 'Củ đều, không mềm ở phần gốc.'],
    avoidSigns: ['Củ mốc, mềm nhũn hoặc chảy nước.', 'Mùi thối, ẩm mốc hoặc chua.', 'Mọc mầm dài nhiều và phần củ teo.', 'Vỏ ướt, dính bết hoặc có côn trùng.'],
    storage: ['Để nơi khô thoáng, tránh túi kín ẩm.', 'Không để trong tủ lạnh ẩm khi còn nguyên củ khô.', 'Tránh ánh nắng trực tiếp.', 'Hành đã bóc nên cho hộp kín và để ngăn mát.'],
    safetyNote: 'Bỏ phần mốc hoặc hư. Nếu mốc lan rộng, bỏ cả củ thay vì chỉ cắt bên ngoài.'
  },
  {
    id: 28,
    nameVi: 'Tỏi',
    nameEn: 'Garlic',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/toi.jpg',
    shortDesc: 'Gia vị thơm dùng trong món xào, nướng, nước chấm, sốt và ướp.',
    chooseTips: ['Củ chắc, tép đầy, vỏ khô.', 'Không có mùi thối hoặc ẩm mốc.', 'Tép tỏi không mềm nhũn khi bóp nhẹ.', 'Mọc mầm ít nếu dùng ngay vẫn có thể chấp nhận.'],
    avoidSigns: ['Tỏi mốc, mềm, chảy nước hoặc tép đen.', 'Mùi thối, chua hoặc ẩm mốc.', 'Củ teo khô quá nhiều, tép rỗng.', 'Tỏi ngâm dầu để lâu ở nhiệt độ phòng không rõ ngày làm.'],
    storage: ['Để nơi khô, thoáng, tránh nắng.', 'Không cất tỏi khô nguyên củ trong môi trường quá ẩm.', 'Tỏi bóc vỏ nên để hộp kín trong ngăn mát và dùng sớm.', 'Không dùng muỗng/dao ướt cho lọ tỏi khô.'],
    safetyNote: 'Không bảo quản tỏi ngâm dầu ở nhiệt độ phòng lâu vì có nguy cơ không an toàn.'
  },
  {
    id: 29,
    nameVi: 'Gừng',
    nameEn: 'Ginger',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/gung.jpg',
    shortDesc: 'Gia vị cay ấm dùng khử mùi thịt cá, kho gà, hấp hải sản hoặc pha trà.',
    chooseTips: ['Củ chắc, vỏ mỏng vừa, không teo nhiều.', 'Mùi gừng thơm cay tự nhiên.', 'Bẻ nhẹ thấy ruột tươi, không xơ khô quá mức.', 'Không có mốc ở các khe củ.'],
    avoidSigns: ['Gừng mềm nhũn, chảy nước hoặc mốc xanh/đen.', 'Mùi lạ, mùi chua hoặc ẩm mốc.', 'Củ quá khô teo, ruột xơ và ít mùi.', 'Vết thối lan trong các nhánh nhỏ.'],
    storage: ['Để nơi khô thoáng nếu dùng nhanh.', 'Có thể bọc kín và để ngăn mát để giữ lâu hơn.', 'Gừng đã cắt nên dùng sớm hoặc bọc kín.', 'Không để gừng ướt trong túi kín lâu.'],
    safetyNote: 'Rửa sạch đất bám trước khi cắt. Bỏ củ nếu mốc lan rộng vào nhiều khe.'
  },
  {
    id: 30,
    nameVi: 'Nấm khô',
    nameEn: 'Dried Mushroom',
    category: 'gia-vi-thuc-pham-kho',
    categoryLabel: 'Gia vị & thực phẩm khô',
    image: '/assets/images/food-guide/gia-vi-thuc-pham-kho/nam-kho.jpg',
    shortDesc: 'Thực phẩm khô tạo mùi thơm và vị ngọt cho canh, lẩu, món chay, món hầm.',
    chooseTips: ['Nấm khô đều màu, không vụn nát quá nhiều.', 'Mùi thơm đặc trưng, không ẩm mốc.', 'Bao bì kín, khô và có nguồn gốc rõ.', 'Tai nấm khô giòn vừa, không dính bết.'],
    avoidSigns: ['Nấm có mùi mốc, mùi ẩm hoặc mùi lạ.', 'Có đốm mốc trắng/xanh/đen bất thường.', 'Túi nấm có mọt, ẩm hoặc bụi bẩn nhiều.', 'Nấm mềm ỉu, dính lại thành cục.'],
    storage: ['Để hộp kín nơi khô ráo.', 'Tránh ánh nắng trực tiếp và nơi ẩm.', 'Sau khi mở túi, buộc kín hoặc hút ẩm nếu có.', 'Nấm đã ngâm nên dùng trong ngày.'],
    safetyNote: 'Ngâm rửa kỹ trước khi nấu. Không dùng nấm khô bị mốc vì độc tố nấm mốc có thể không mất hết khi nấu.'
  },
  {
    id: 31,
    nameVi: 'Cá ngừ',
    nameEn: 'Tuna',
    category: 'ca-hai-san',
    categoryLabel: 'Cá & hải sản',
    image: '/assets/images/food-guide/ca-hai-san/ca-ngu.jpg',
    shortDesc: 'Cá thịt chắc, dùng áp chảo, kho, làm salad chín hoặc món cơm healthy.',
    chooseTips: ['Thịt cá đỏ hồng hoặc hồng sậm tự nhiên, không xỉn xám.', 'Miếng cá chắc, thớ thịt liền, không bở vụn.', 'Bề mặt hơi ẩm nhưng không nhớt.', 'Mùi biển nhẹ, không tanh gắt hoặc khai.'],
    avoidSigns: ['Thịt cá nâu xám, tái nhợt hoặc chảy dịch đục.', 'Mùi hôi, khai, chua hoặc tanh nồng.', 'Miếng cá mềm bở, rã nước nhiều.', 'Cá đông lạnh có lớp đá dày và dấu hiệu rã đông lại.'],
    storage: ['Giữ lạnh liên tục sau khi mua.', 'Dùng trong ngày nếu mua cá tươi.', 'Cấp đông trong túi kín nếu chưa chế biến.', 'Rã đông trong ngăn mát và nấu sớm sau khi rã.'],
    safetyNote: 'Nếu không phải cá đạt chuẩn ăn sống, nên nấu chín. Người dị ứng cá/hải sản nên tránh.'
  },
  {
    id: 32,
    nameVi: 'Bắp cải',
    nameEn: 'Cabbage',
    category: 'rau-cu',
    categoryLabel: 'Rau củ',
    image: '/assets/images/food-guide/rau-cu/bap-cai.jpg',
    shortDesc: 'Rau cuộn lá dùng để luộc, xào, nấu canh, làm salad hoặc món cuốn.',
    chooseTips: ['Bắp cải cầm nặng tay, cuộn lá chắc vừa.', 'Lá ngoài xanh hoặc trắng tự nhiên tùy loại, không héo nhiều.', 'Phần cuống khô sạch, không thâm đen.', 'Lá không có mùi chua hoặc nhớt.'],
    avoidSigns: ['Bắp cải mềm xốp, nhẹ bất thường hoặc lá rời nhiều.', 'Cuống thối đen, chảy nước hoặc có mốc.', 'Lá ngoài nhớt, vàng úa lan rộng.', 'Có mùi chua, mùi lên men hoặc vết sâu hỏng lớn.'],
    storage: ['Giữ nguyên bắp, bọc lỏng rồi để ngăn mát.', 'Chỉ rửa phần lá cần dùng ngay trước khi chế biến.', 'Phần đã cắt nên bọc kín mặt cắt.', 'Dùng phần đã cắt trong vài ngày để giữ độ giòn.'],
    safetyNote: 'Tách từng lớp lá và rửa kỹ vì đất/côn trùng có thể nằm giữa các bẹ lá.'
  }
];

let activeFoodGuideCategory = 'all';
let foodGuideSearchTerm = '';
let apiFoodGuideItems = [];
let apiFoodGuideLoaded = false;

const FOOD_GUIDE_LOCAL_IMAGES = {
  6: '/assets/images/food-guide/rau-cu/bi-do.jpg',
  9: '/assets/images/food-guide/trai-cay/chuoi.jpg',
  10: '/assets/images/food-guide/trai-cay/bo.jpg',
  11: '/assets/images/food-guide/trai-cay/dua-hau.jpg',
  12: '/assets/images/food-guide/trai-cay/xoai.jpg',
  13: '/assets/images/food-guide/thit/thit-heo.jpg',
  14: '/assets/images/food-guide/thit/thit-bo.jpg',
  15: '/assets/images/food-guide/thit/thit-ga.jpg',
  16: '/assets/images/food-guide/thit/suon-heo.jpg',
  24: '/assets/images/food-guide/dau-hat-ngu-coc/dau-nanh.jpg',
  26: '/assets/images/food-guide/dau-hat-ngu-coc/yen-mach.jpg'
};

const FOOD_GUIDE_CATEGORY_FALLBACK_IMAGES = {
  'rau-cu': '/assets/images/food-guide/rau-cu/bi-do.jpg',
  'trai-cay': '/assets/images/food-guide/trai-cay/bo.jpg',
  'thit': '/assets/images/food-guide/thit/thit-heo.jpg',
  'ca-hai-san': '/assets/images/menu/mon-man/ca-hoi-ap-chao.jpg',
  'trung-sua': '/assets/images/recipes/mon-man/trung-chien-hanh.jpg',
  'dau-hat-ngu-coc': '/assets/images/food-guide/dau-hat-ngu-coc/dau-nanh.jpg',
  'gia-vi-thuc-pham-kho': '/assets/images/recipes/mon-man/ga-kho-gung.jpg'
};

function getFoodGuideSource() {
  return FOOD_GUIDE_DETAILED_ITEMS;
}

function getFoodGuideItemImage(item) {
  return FOOD_GUIDE_LOCAL_IMAGES[item.id]
    || item.image
    || FOOD_GUIDE_CATEGORY_FALLBACK_IMAGES[item.category]
    || '/assets/images/food-guide/placeholder.jpg';
}

function normalizeFoodGuideText(value) {
  return String(value || '')
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd');
}

function getFilteredFoodGuideItems() {
  const keyword = normalizeFoodGuideText(foodGuideSearchTerm);

  return getFoodGuideSource().filter(item => {
    const matchCategory = activeFoodGuideCategory === 'all' || item.category === activeFoodGuideCategory;
    const searchable = normalizeFoodGuideText(`${item.nameVi} ${item.nameEn} ${item.categoryLabel}`);
    return matchCategory && (!keyword || searchable.includes(keyword));
  });
}

function foodGuideCardTemplate(item) {
  return `
    <article class="food-guide-card">
      <button type="button" class="food-guide-card-button" onclick="openFoodGuideModal(${item.id})">
        <div class="food-guide-card-image">
          <img src="${escapeHtml(getFoodGuideItemImage(item))}" alt="${escapeHtml(item.nameVi)}" />
          <span class="food-guide-badge food-guide-badge-${escapeHtml(item.category)}">${escapeHtml(item.categoryLabel)}</span>
        </div>
        <div class="food-guide-card-body">
          <p>${escapeHtml(item.nameEn)}</p>
          <h3>${escapeHtml(item.nameVi)}</h3>
          <span>${escapeHtml(item.shortDesc)}</span>
          <strong>Xem mẹo chọn</strong>
        </div>
      </button>
    </article>
  `;
}

function renderFoodGuideItems() {
  const grid = document.getElementById('foodGuideGrid');
  const resultText = document.getElementById('foodGuideResultText');
  if (!grid) return;

  const items = getFilteredFoodGuideItems();
  grid.innerHTML = items.length
    ? items.map(foodGuideCardTemplate).join('')
    : '<p class="food-guide-empty">Không tìm thấy thực phẩm phù hợp.</p>';

  if (resultText) {
    const categoryLabel = activeFoodGuideCategory === 'all'
      ? 'tất cả nhóm'
      : document.querySelector(`.food-guide-filters [data-category="${activeFoodGuideCategory}"]`)?.textContent || activeFoodGuideCategory;
    const searchText = foodGuideSearchTerm ? `, từ khóa "${foodGuideSearchTerm}"` : '';
    resultText.textContent = `Đang hiển thị ${items.length}/${getFoodGuideSource().length} thực phẩm thuộc ${categoryLabel}${searchText}.`;
  }
}

function setFoodGuideList(id, values) {
  const element = document.getElementById(id);
  if (!element) return;
  element.innerHTML = values.map(value => `<li>${escapeHtml(value)}</li>`).join('');
}

function openFoodGuideModal(id) {
  const item = getFoodGuideSource().find(food => food.id === id);
  const modal = document.getElementById('foodGuideModal');
  if (!item || !modal) return;

  document.getElementById('foodGuideModalImage').src = getFoodGuideItemImage(item);
  document.getElementById('foodGuideModalImage').alt = item.nameVi;
  document.getElementById('foodGuideModalCategory').textContent = item.categoryLabel;
  document.getElementById('foodGuideModalNameEn').textContent = item.nameEn;
  document.getElementById('foodGuideModalNameVi').textContent = item.nameVi;
  document.getElementById('foodGuideModalDesc').textContent = item.shortDesc;
  setFoodGuideList('foodGuideModalChooseTips', item.chooseTips);
  setFoodGuideList('foodGuideModalAvoidSigns', item.avoidSigns);
  setFoodGuideList('foodGuideModalStorage', item.storage);
  document.getElementById('foodGuideModalSafety').textContent = item.safetyNote;

  modal.classList.add('show');
  modal.setAttribute('aria-hidden', 'false');
  document.body.classList.add('modal-open');
}

function closeFoodGuideModal() {
  const modal = document.getElementById('foodGuideModal');
  if (!modal) return;
  modal.classList.remove('show');
  modal.setAttribute('aria-hidden', 'true');
  document.body.classList.remove('modal-open');
}

function normalizeFoodGuideItemFromApi(item) {
  return {
    id: item.id,
    nameVi: item.nameVi || '',
    nameEn: item.nameEn || '',
    category: item.category || 'rau-cu',
    categoryLabel: item.categoryLabel || 'Rau củ',
    image: item.image || '/assets/images/food-guide/placeholder.jpg',
    shortDesc: item.shortDesc || '',
    chooseTips: Array.isArray(item.chooseTips) ? item.chooseTips : [],
    avoidSigns: Array.isArray(item.avoidSigns) ? item.avoidSigns : [],
    storage: Array.isArray(item.storage) ? item.storage : [],
    safetyNote: item.safetyNote || ''
  };
}

async function loadFoodGuideFromApi() {
  apiFoodGuideItems = FOOD_GUIDE_DETAILED_ITEMS;
  apiFoodGuideLoaded = true;
}

document.addEventListener('DOMContentLoaded', async () => {
  await loadFoodGuideFromApi();

  document.querySelectorAll('.food-guide-filters button').forEach(button => {
    button.addEventListener('click', () => {
      document.querySelectorAll('.food-guide-filters button').forEach(item => item.classList.remove('active'));
      button.classList.add('active');
      activeFoodGuideCategory = button.dataset.category || 'all';
      renderFoodGuideItems();
    });
  });

  const searchInput = document.getElementById('foodGuideSearchInput');
  if (searchInput) {
    searchInput.addEventListener('input', () => {
      foodGuideSearchTerm = searchInput.value;
      renderFoodGuideItems();
    });
  }

  renderFoodGuideItems();
});

document.addEventListener('keydown', event => {
  if (event.key === 'Escape') {
    closeFoodGuideModal();
  }
});
