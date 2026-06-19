const MENU_ALLERGY_NOTE = 'Món có thể chứa hải sản, đậu nành, sữa, trứng, gluten, mè hoặc đậu phộng. Nếu quý khách dị ứng với bất kỳ thành phần nào, chúng tôi khuyến nghị nên chọn món khác hoặc liên hệ nhân viên để được tư vấn.';

const MENU_FALLBACK_IMAGE = '/images/foods/fallback-food.jpg';
const MENU_MAX_QUANTITY = 20;

const MENU_CATEGORIES = {
  'Món mặn': ['Cơm phần', 'Món nước', 'Món xào', 'Món gà', 'Món cá'],
  'Món chay': ['Cơm chay', 'Món nước chay', 'Món đậu', 'Món cuốn', 'Món lẩu'],
  'Healthy': ['Salad', 'Cơm healthy', 'Protein bowl', 'Soup', 'Ăn nhẹ'],
  'Đồ uống': ['Trà', 'Nước ép', 'Sinh tố', 'Sữa hạt', 'Healthy drink']
};

const DEDICATED_MENU_ITEMS = [
  {
    id: 201,
    name: 'Cơm gà xối mỡ',
    category: 'Món mặn',
    subcategory: 'Món gà',
    price: 69000,
    image: '/assets/images/menu/mon-man/com-ga-xoi-mo.jpg',
    description: 'Cơm nóng dùng với gà da giòn, rau dưa và nước mắm chua ngọt.',
    story: 'Món cơm gà xối mỡ được chọn cho những bữa trưa cần no bụng nhưng vẫn quen vị Việt. Lớp da giòn giúp món ăn hấp dẫn hơn khi dùng tại quán.',
    ingredients: ['Cơm trắng', 'Đùi gà', 'Dưa leo', 'Rau thơm', 'Nước mắm chua ngọt'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 202,
    name: 'Cơm sườn nướng',
    category: 'Món mặn',
    subcategory: 'Cơm phần',
    price: 55000,
    image: '/images/foods/com-suon-nuong.jpg',
    description: 'Cơm nóng ăn kèm sườn heo nướng mềm thơm, trứng ốp la, đồ chua và nước mắm pha.',
    detailDescription: 'Cơm sườn nướng là món ăn quen thuộc với phần sườn heo được tẩm ướp gia vị và nướng vàng thơm. Món ăn dùng cùng cơm trắng, trứng ốp la, dưa leo, cà chua, đồ chua và nước mắm chua ngọt, phù hợp cho bữa trưa hoặc bữa tối.',
    story: 'Cơm sườn nướng là món ăn quen thuộc với phần sườn heo được tẩm ướp gia vị và nướng vàng thơm. Món ăn dùng cùng cơm trắng, trứng ốp la, dưa leo, cà chua, đồ chua và nước mắm chua ngọt, phù hợp cho bữa trưa hoặc bữa tối.',
    ingredients: ['Cơm trắng', 'Sườn heo', 'Trứng gà', 'Dưa leo', 'Cà chua', 'Đồ chua', 'Nước mắm', 'Tỏi', 'Hành tím', 'Đường', 'Tiêu', 'Dầu ăn', 'Gia vị ướp'],
    calories: 650,
    servingSize: '1 người',
    spiceLevel: 'Không cay hoặc cay nhẹ tùy nước mắm.',
    isAvailable: true,
    isFeatured: true,
    isVegetarian: false,
    allergy: 'Có thể chứa trứng gà, nước mắm/cá, tỏi, hành tím và các gia vị ướp. Người dị ứng với trứng, hải sản/cá hoặc gia vị lên men cần lưu ý.'
  },
  {
    id: 203,
    name: 'Bún bò Huế',
    category: 'Món mặn',
    subcategory: 'Món nước',
    price: 79000,
    image: '/assets/images/menu/mon-man/bun-bo-hue.jpg',
    description: 'Bún bò nước dùng đậm vị, có bò, chả và rau ăn kèm.',
    story: 'Bún bò Huế mang lại cảm giác ấm bụng trong những ngày cần món nước đậm đà. Món được cân vị để dễ ăn với nhiều khách.',
    ingredients: ['Bún', 'Thịt bò', 'Chả', 'Sả', 'Rau sống'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 204,
    name: 'Phở bò',
    category: 'Món mặn',
    subcategory: 'Món nước',
    price: 75000,
    image: '/assets/images/menu/mon-man/pho-bo.jpg',
    description: 'Phở bò nước trong, thơm nhẹ, dùng với rau thơm và chanh.',
    story: 'Phở bò là lựa chọn an toàn cho bữa sáng hoặc bữa tối nhẹ. Nước dùng được nấu theo hướng thanh và dễ ăn.',
    ingredients: ['Bánh phở', 'Thịt bò', 'Hành tây', 'Hành lá', 'Rau thơm'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 205,
    name: 'Mì xào hải sản',
    category: 'Món mặn',
    subcategory: 'Món xào',
    price: 85000,
    image: '/assets/images/menu/mon-man/mi-xao-hai-san.jpg',
    description: 'Mì xào cùng tôm, mực và rau củ, vị đậm vừa ăn.',
    story: 'Mì xào hải sản hợp khi khách muốn món nhanh, nhiều rau và có vị biển nhẹ. Món được xào lửa lớn để giữ độ giòn của rau.',
    ingredients: ['Mì', 'Tôm', 'Mực', 'Cải thìa', 'Cà rốt', 'Nước sốt xào'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 206,
    name: 'Gà sốt mật ong',
    category: 'Món mặn',
    subcategory: 'Món gà',
    price: 76000,
    image: '/assets/images/menu/mon-man/ga-sot-mat-ong.jpg',
    description: 'Gà chiên phủ sốt mật ong nhẹ, vị ngọt mặn dễ ăn.',
    story: 'Gà sốt mật ong là món hợp nhóm bạn hoặc bữa ăn gia đình nhỏ. Vị sốt cân bằng để trẻ em và người lớn đều dễ dùng.',
    ingredients: ['Thịt gà', 'Mật ong', 'Mè', 'Tỏi', 'Gia vị'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 207,
    name: 'Cá hồi áp chảo',
    category: 'Món mặn',
    subcategory: 'Món cá',
    price: 129000,
    image: '/assets/images/menu/mon-man/ca-hoi-ap-chao.jpg',
    description: 'Cá hồi áp chảo dùng với rau củ và sốt chanh nhẹ.',
    story: 'Cá hồi áp chảo dành cho khách muốn món mặn nhưng thanh hơn. Phần cá được áp chảo vừa chín để giữ độ mềm.',
    ingredients: ['Cá hồi', 'Bông cải', 'Cà rốt', 'Sốt chanh', 'Tiêu'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 226,
    slug: 'ca-basa-kho-to',
    name: 'Cá basa kho tộ',
    category: 'Món mặn',
    subcategory: 'Món cá',
    price: 78000,
    image: '/assets/images/menu/mon-man/ca-basa-kho-to.jpg',
    description: 'Cá basa kho tộ vị đậm, dùng cùng cơm nóng và rau luộc.',
    story: 'Cá basa kho tộ là món cơm gia đình quen thuộc, hợp khách muốn món mặn dễ ăn, nước kho vừa đủ để ăn kèm cơm.',
    ingredients: ['Cá basa', 'Nước mắm', 'Tiêu', 'Hành tím', 'Ớt', 'Nước màu'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 208,
    name: 'Cơm chay thập cẩm',
    category: 'Món chay',
    subcategory: 'Cơm chay',
    price: 59000,
    image: '/assets/images/menu/mon-chay/com-chay-thap-cam.jpg',
    description: 'Cơm chay với rau củ, nấm và đậu hũ sốt nhẹ.',
    story: 'Cơm chay thập cẩm phù hợp ngày rằm hoặc những hôm muốn ăn thanh. Món tập trung vào rau củ tươi và vị tự nhiên.',
    ingredients: ['Cơm trắng', 'Đậu hũ', 'Nấm', 'Cà rốt', 'Rau cải'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 209,
    name: 'Bún chay nấm',
    category: 'Món chay',
    subcategory: 'Món nước chay',
    price: 62000,
    image: '/assets/images/menu/mon-chay/bun-chay-nam.jpg',
    description: 'Bún nước chay nấu từ nấm, rau củ và đậu hũ.',
    story: 'Bún chay nấm mang lại cảm giác ấm và nhẹ bụng. Nước dùng lấy vị ngọt từ rau củ thay vì gia vị nặng.',
    ingredients: ['Bún', 'Nấm rơm', 'Nấm kim châm', 'Đậu hũ', 'Rau thơm'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 210,
    name: 'Đậu hũ sốt cà',
    category: 'Món chay',
    subcategory: 'Món đậu',
    price: 49000,
    image: '/assets/images/menu/mon-chay/dau-hu-sot-ca.jpg',
    description: 'Đậu hũ mềm nấu cùng sốt cà chua tươi.',
    story: 'Đậu hũ sốt cà là món chay gần gũi, dễ ăn và hợp với cơm nóng. Sốt cà tạo vị chua nhẹ giúp món không ngấy.',
    ingredients: ['Đậu hũ', 'Cà chua', 'Hành lá', 'Dầu thực vật', 'Gia vị chay'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 211,
    name: 'Gỏi cuốn chay',
    category: 'Món chay',
    subcategory: 'Món cuốn',
    price: 52000,
    image: '/assets/images/menu/mon-chay/goi-cuon-chay.jpg',
    description: 'Gỏi cuốn rau củ, bún, đậu hũ và nước chấm chay.',
    story: 'Gỏi cuốn chay hợp cho bữa nhẹ hoặc khai vị. Món giữ độ tươi của rau và tạo cảm giác mát, dễ chịu.',
    ingredients: ['Bánh tráng', 'Bún', 'Đậu hũ', 'Xà lách', 'Rau thơm'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 212,
    name: 'Mì xào rau củ',
    category: 'Món chay',
    subcategory: 'Món nước chay',
    price: 58000,
    image: '/assets/images/menu/mon-chay/mi-xao-rau-cu.jpg',
    description: 'Mì xào chay với nấm, cải, cà rốt và sốt đậu nành.',
    story: 'Mì xào rau củ dành cho khách muốn món chay nhanh nhưng vẫn đủ năng lượng. Rau được xào vừa chín để giữ độ giòn.',
    ingredients: ['Mì', 'Nấm', 'Cải thìa', 'Cà rốt', 'Nước tương'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 213,
    name: 'Lẩu nấm chay',
    category: 'Món chay',
    subcategory: 'Món lẩu',
    price: 159000,
    image: '/assets/images/menu/mon-chay/lau-nam-chay.jpg',
    description: 'Lẩu chay nhiều loại nấm, rau xanh và đậu hũ.',
    story: 'Lẩu nấm chay phù hợp đi nhóm, đặc biệt trong những ngày muốn ăn thanh đạm. Nước lẩu được nấu từ rau củ và nấm.',
    ingredients: ['Nấm tổng hợp', 'Rau xanh', 'Đậu hũ', 'Bắp', 'Mì hoặc bún'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 214,
    name: 'Salad ức gà',
    category: 'Healthy',
    subcategory: 'Salad',
    price: 69000,
    image: '/assets/images/menu/healthy/salad-uc-ga.jpg',
    description: 'Salad rau xanh với ức gà áp chảo và sốt mè rang.',
    story: 'Salad ức gà là lựa chọn cân bằng cho khách muốn ăn nhẹ nhưng vẫn đủ protein. Món hợp bữa trưa văn phòng.',
    ingredients: ['Ức gà', 'Xà lách', 'Cà chua bi', 'Dưa leo', 'Sốt mè rang'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 215,
    name: 'Cơm gạo lứt ức gà',
    category: 'Healthy',
    subcategory: 'Cơm healthy',
    price: 79000,
    image: '/assets/images/menu/healthy/com-gao-lut-uc-ga.jpg',
    description: 'Gạo lứt, ức gà, rau luộc và sốt nhẹ.',
    story: 'Cơm gạo lứt ức gà dành cho khách theo chế độ ăn lành mạnh. Món ưu tiên nguyên liệu đơn giản, ít dầu.',
    ingredients: ['Gạo lứt', 'Ức gà', 'Bông cải', 'Cà rốt', 'Sốt sữa chua'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 216,
    name: 'Bowl cá hồi',
    category: 'Healthy',
    subcategory: 'Protein bowl',
    price: 119000,
    image: '/assets/images/menu/healthy/bowl-ca-hoi.jpg',
    description: 'Bowl cá hồi cùng rau củ, bơ và cơm nền nhẹ.',
    story: 'Bowl cá hồi kết hợp protein, chất béo tốt và rau củ. Đây là lựa chọn hợp cho khách muốn ăn no nhưng không nặng bụng.',
    ingredients: ['Cá hồi', 'Cơm nền', 'Bơ', 'Dưa leo', 'Rong biển'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 217,
    name: 'Salad cá ngừ',
    category: 'Healthy',
    subcategory: 'Salad',
    price: 74000,
    image: '/assets/images/menu/healthy/salad-ca-ngu.jpg',
    description: 'Salad cá ngừ, rau xanh, bắp và sốt chanh dầu olive.',
    story: 'Salad cá ngừ phù hợp khi cần món nhiều protein và tươi. Vị chanh nhẹ giúp món ăn sáng miệng hơn.',
    ingredients: ['Cá ngừ', 'Xà lách', 'Bắp ngọt', 'Cà chua', 'Dầu olive'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 218,
    name: 'Yến mạch trái cây',
    category: 'Healthy',
    subcategory: 'Ăn nhẹ',
    price: 55000,
    image: '/assets/images/menu/healthy/yen-mach-trai-cay.jpg',
    description: 'Yến mạch dùng với sữa chua, chuối và trái cây theo mùa.',
    story: 'Yến mạch trái cây là món nhẹ cho buổi sáng hoặc xế chiều. Món có vị ngọt tự nhiên từ trái cây.',
    ingredients: ['Yến mạch', 'Sữa chua', 'Chuối', 'Dâu', 'Hạt dinh dưỡng'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 219,
    name: 'Soup bí đỏ',
    category: 'Healthy',
    subcategory: 'Soup',
    price: 59000,
    image: '/assets/images/menu/healthy/soup-bi-do.jpg',
    description: 'Soup bí đỏ mịn, ấm bụng, dùng kèm bánh mì nướng.',
    story: 'Soup bí đỏ hợp khi khách muốn món ấm và nhẹ. Bí đỏ tạo độ ngọt tự nhiên, không cần nêm quá nhiều.',
    ingredients: ['Bí đỏ', 'Sữa tươi', 'Hành tây', 'Bánh mì', 'Tiêu'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 220,
    name: 'Trà đào cam sả',
    category: 'Đồ uống',
    subcategory: 'Trà',
    price: 45000,
    image: '/assets/images/menu/do-uong/tra-dao-cam-sa.png',
    description: 'Trà đào thơm cam sả, vị chua ngọt dễ uống.',
    story: 'Trà đào cam sả là món nước giải nhiệt hợp với các món chiên hoặc món cay. Hương sả tạo cảm giác tươi hơn.',
    ingredients: ['Trà', 'Đào', 'Cam', 'Sả', 'Đường'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 221,
    name: 'Trà chanh mật ong',
    category: 'Đồ uống',
    subcategory: 'Trà',
    price: 39000,
    image: '/assets/images/menu/do-uong/tra-chanh-mat-ong.jpg',
    description: 'Trà chanh pha mật ong, vị thanh và thơm nhẹ.',
    story: 'Trà chanh mật ong là lựa chọn nhẹ nhàng sau bữa ăn. Vị chanh giúp cân bằng các món nhiều đạm.',
    ingredients: ['Trà', 'Chanh', 'Mật ong', 'Đá', 'Lá bạc hà'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 222,
    name: 'Nước ép cam',
    category: 'Đồ uống',
    subcategory: 'Nước ép',
    price: 42000,
    image: '/assets/images/menu/do-uong/nuoc-ep-cam.jpg',
    description: 'Nước cam ép tươi, có thể giảm đường theo yêu cầu.',
    story: 'Nước ép cam là món uống quen thuộc, hợp dùng kèm bữa sáng hoặc sau món mặn để làm dịu vị.',
    ingredients: ['Cam tươi', 'Đá', 'Đường tùy chọn'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 223,
    name: 'Sinh tố bơ',
    category: 'Đồ uống',
    subcategory: 'Sinh tố',
    price: 49000,
    image: '/assets/images/menu/do-uong/sinh-to-bo.jpg',
    description: 'Sinh tố bơ béo nhẹ, xay mịn, vị ngọt vừa.',
    story: 'Sinh tố bơ phù hợp khi khách muốn món uống no nhẹ. Bơ chín tạo độ mịn tự nhiên và cảm giác dễ chịu.',
    ingredients: ['Bơ', 'Sữa đặc', 'Sữa tươi', 'Đá'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 224,
    name: 'Sữa đậu nành',
    category: 'Đồ uống',
    subcategory: 'Sữa hạt',
    price: 35000,
    image: '/assets/images/menu/do-uong/sua-dau-nanh.jpg',
    description: 'Sữa đậu nành thơm nhẹ, dùng nóng hoặc lạnh.',
    story: 'Sữa đậu nành là món uống đơn giản, hợp bữa sáng hoặc món chay. Vị được giữ vừa phải để dễ dùng.',
    ingredients: ['Đậu nành', 'Nước lọc', 'Đường tùy chọn'],
    allergy: MENU_ALLERGY_NOTE
  },
  {
    id: 225,
    name: 'Nước ép cần tây táo',
    category: 'Đồ uống',
    subcategory: 'Healthy drink',
    price: 52000,
    image: '/assets/images/menu/do-uong/nuoc-ep-can-tay-tao.jpg',
    description: 'Nước ép cần tây kết hợp táo, vị xanh nhẹ và dễ uống.',
    story: 'Nước ép cần tây táo dành cho khách thích đồ uống healthy. Táo giúp vị cần tây mềm hơn và dễ tiếp cận.',
    ingredients: ['Cần tây', 'Táo', 'Chanh', 'Đá'],
    allergy: MENU_ALLERGY_NOTE
  }
];

let activeMenuCategory = 'all';
let activeMenuSubcategory = 'all';
let activeMenuKeyword = '';
let apiMenuItems = [];
let apiMenuLoaded = false;

const MENU_LOCAL_IMAGES = {
  'com-ga-sot-tieu-den': '/assets/images/menu/mon-man/com-ga-sot-tieu-den.jpg',
  'com-suon-nuong': '/images/foods/com-suon-nuong.jpg',
  'ga-sot-mat-ong': '/assets/images/menu/mon-man/ga-sot-mat-ong.jpg',
  'ga-chien-mat-ong': '/assets/images/menu/mon-man/ga-chien-mat-ong.jpg',
  'mi-bo-cay': '/assets/images/menu/mon-man/mi-bo-cay.jpg',
  'burger-bo-pho-mai': '/assets/images/menu/fastfood/burger-bo-pho-mai.jpg',
  'com-chay-nam': '/assets/images/menu/mon-chay/com-chay-nam.jpg',
  'salad-uc-ga': '/assets/images/menu/healthy/salad-uc-ga.jpg',
  'salad-uc-ga-menu': '/assets/images/menu/healthy/salad-uc-ga.jpg',
  'salad-ca-ngu': '/assets/images/menu/healthy/salad-ca-ngu.jpg',
  'yen-mach-trai-cay': '/assets/images/menu/healthy/yen-mach-trai-cay.jpg',
  'soup-bi-do': '/assets/images/menu/healthy/soup-bi-do.jpg',
  'ca-basa-kho-to': '/assets/images/menu/mon-man/ca-basa-kho-to.jpg',
  'tra-chanh-mat-ong': '/assets/images/menu/do-uong/tra-chanh-mat-ong.jpg',
  'sinh-to-bo': '/assets/images/menu/do-uong/sinh-to-bo.jpg',
  'sua-dau-nanh': '/assets/images/menu/do-uong/sua-dau-nanh.jpg'
};

const MENU_LOCAL_IMAGES_BY_NAME = {
  'com ga sot tieu den': '/assets/images/menu/mon-man/com-ga-sot-tieu-den.jpg',
  'com suon nuong': '/images/foods/com-suon-nuong.jpg',
  'ga sot mat ong': '/assets/images/menu/mon-man/ga-sot-mat-ong.jpg',
  'ga chien mat ong': '/assets/images/menu/mon-man/ga-chien-mat-ong.jpg',
  'mi bo cay': '/assets/images/menu/mon-man/mi-bo-cay.jpg',
  'burger bo pho mai': '/assets/images/menu/fastfood/burger-bo-pho-mai.jpg',
  'com chay nam': '/assets/images/menu/mon-chay/com-chay-nam.jpg',
  'salad uc ga': '/assets/images/menu/healthy/salad-uc-ga.jpg',
  'salad ca ngu': '/assets/images/menu/healthy/salad-ca-ngu.jpg',
  'yen mach trai cay': '/assets/images/menu/healthy/yen-mach-trai-cay.jpg',
  'soup bi do': '/assets/images/menu/healthy/soup-bi-do.jpg',
  'ca basa kho to': '/assets/images/menu/mon-man/ca-basa-kho-to.jpg',
  'tra chanh mat ong': '/assets/images/menu/do-uong/tra-chanh-mat-ong.jpg',
  'sinh to bo': '/assets/images/menu/do-uong/sinh-to-bo.jpg',
  'sua dau nanh': '/assets/images/menu/do-uong/sua-dau-nanh.jpg'
};

function formatMenuPrice(value) {
  return Number(value).toLocaleString('vi-VN') + 'đ';
}

function getMenuSource() {
  if (!apiMenuLoaded || !apiMenuItems.length) return DEDICATED_MENU_ITEMS;

  const apiSlugs = new Set(apiMenuItems.map(item => item.slug).filter(Boolean));
  const apiNames = new Set(apiMenuItems.map(item => normalizeMenuImageKey(item.name)));
  const missingDedicatedItems = DEDICATED_MENU_ITEMS.filter(item =>
    (item.slug && !apiSlugs.has(item.slug)) ||
    (!item.slug && !apiNames.has(normalizeMenuImageKey(item.name))));

  return [...apiMenuItems, ...missingDedicatedItems];
}

function normalizeMenuImageKey(value) {
  return String(value || '')
    .trim()
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd');
}

function getMenuItemImage(item) {
  const slugImage = MENU_LOCAL_IMAGES[item.slug];
  if (slugImage) return slugImage;

  const nameImage = MENU_LOCAL_IMAGES_BY_NAME[normalizeMenuImageKey(item.name)];
  if (nameImage) return nameImage;

  return item.image || MENU_FALLBACK_IMAGE;
}

function getMenuDetailUrl(item) {
  const slug = item.slug || normalizeMenuImageKey(item.name).replaceAll(' ', '-');
  return `/menu/${encodeURIComponent(slug)}`;
}

function menuPriceTemplate(item) {
  const hasDiscount = item.discountPrice && Number(item.discountPrice) > 0 && Number(item.discountPrice) < Number(item.price);
  if (hasDiscount) {
    return `<strong>${formatMenuPrice(item.discountPrice)}</strong><del>${formatMenuPrice(item.price)}</del>`;
  }

  return `<strong>${formatMenuPrice(item.price)}</strong>`;
}

function clampMenuQuantity(value) {
  const quantity = Number.parseInt(value, 10);
  if (Number.isNaN(quantity)) return 1;
  return Math.min(Math.max(quantity, 1), MENU_MAX_QUANTITY);
}

function getMenuQuantity(itemId) {
  const input = document.getElementById(`menuQty-${itemId}`);
  return clampMenuQuantity(input ? input.value : 1);
}

function changeMenuQuantity(itemId, delta) {
  const input = document.getElementById(`menuQty-${itemId}`);
  if (!input) return;
  input.value = clampMenuQuantity(Number(input.value || 1) + delta);
}

function normalizeMenuQuantity(input) {
  if (!input) return;
  input.value = clampMenuQuantity(input.value);
}

function menuAddToCart(item, quantity = 1) {
  if (!item || item.isAvailable === false) return;
  if (typeof addToCart === 'function') {
    addToCart(item.name, item.discountPrice || item.price, getMenuItemImage(item), item.id, quantity);
  }
}

function openMenuCardDetail(card) {
  const detailUrl = card?.dataset?.detailUrl;
  if (!detailUrl) return;
  window.location.href = detailUrl;
}

function getFilteredMenuItems() {
  const keyword = normalizeMenuImageKey(activeMenuKeyword);
  const sourceItems = getMenuSource();
  return sourceItems.filter(item => {
    const matchCategory = activeMenuCategory === 'all' || item.category === activeMenuCategory;
    const matchSubcategory = activeMenuSubcategory === 'all' || item.subcategory === activeMenuSubcategory;
    const searchText = normalizeMenuImageKey([
      item.name,
      item.category,
      item.subcategory,
      item.description,
      item.story,
      Array.isArray(item.ingredients) ? item.ingredients.join(' ') : item.ingredients
    ].join(' '));
    const matchKeyword = !keyword || searchText.includes(keyword);
    return matchCategory && matchSubcategory && matchKeyword;
  });
}

function mapApiMenuItem(item) {
  return {
    id: item.id,
    slug: item.slug || '',
    name: item.name,
    category: item.mainCategory || item.category,
    subcategory: item.subcategory || item.tag || 'Khác',
    price: item.price,
    discountPrice: item.discountPrice,
    image: item.imageUrl,
    description: item.description,
    detailDescription: item.detailDescription || item.story || item.description,
    story: item.story,
    ingredients: String(item.ingredients || '').split(',').map(x => x.trim()).filter(Boolean),
    calories: item.calories || 0,
    servingSize: item.servingSize || '1 người',
    spiceLevel: item.spiceLevel || 'Không cay',
    isAvailable: item.isAvailable !== false,
    isFeatured: item.isFeatured || item.isBestSeller,
    isVegetarian: item.isVegetarian || item.mainCategory === 'Món chay',
    allergy: item.allergens || item.allergyNote || MENU_ALLERGY_NOTE
  };
}

async function loadMenuItemsFromApi() {
  try {
    const pageSize = 200;
    let page = 1;
    let totalPages = 1;
    const items = [];

    do {
      const response = await fetch(`/api/foods?page=${page}&pageSize=${pageSize}`, { cache: 'no-store' });
      if (!response.ok) throw new Error('Cannot load menu API');
      const data = await response.json();
      if (Array.isArray(data.items)) {
        items.push(...data.items.map(mapApiMenuItem));
      }

      totalPages = Number(data.totalPages || 1);
      page += 1;
    } while (page <= totalPages);

    apiMenuItems = items;
    apiMenuLoaded = apiMenuItems.length > 0;
  } catch {
    apiMenuItems = [];
    apiMenuLoaded = false;
  }
}

function renderMenuCategoryNav() {
  activeMenuSubcategory = 'all';
}

function menuCardTemplate(item) {
  const detailUrl = getMenuDetailUrl(item);
  return `
    <article class="menu-card" data-detail-url="${escapeHtml(detailUrl)}" onclick="openMenuCardDetail(this)" tabindex="0" role="link" aria-label="Xem chi tiết ${escapeHtml(item.name)}">
      <div class="menu-card-open">
        <div class="menu-card-image">
          <img src="${escapeHtml(getMenuItemImage(item))}" alt="${escapeHtml(item.name)}" onerror="this.onerror=null;this.src='${MENU_FALLBACK_IMAGE}';" />
          <span>${escapeHtml(item.subcategory)}</span>
        </div>
        <div class="menu-card-body">
          <span class="menu-feature-badge">Món ăn</span>
          <div class="menu-card-meta">
            <p>${escapeHtml(item.category)}</p>
            <em class="${item.isAvailable === false ? 'unavailable' : 'available'}">${item.isAvailable === false ? 'Hết món' : 'Còn món'}</em>
          </div>
          <h3>${escapeHtml(item.name)}</h3>
          <span class="menu-card-description">${escapeHtml(item.description)}</span>
          <div class="menu-card-badges">
            ${item.isFeatured ? '<b>Món nổi bật</b>' : ''}
            ${item.isVegetarian ? '<b class="vegetarian">Món chay</b>' : ''}
          </div>
          <div class="menu-card-bottom">
            <div class="menu-card-price">${menuPriceTemplate(item)}</div>
          </div>
          <div class="menu-card-actions">
            <a class="small-btn neutral" href="${escapeHtml(detailUrl)}" onclick="event.stopPropagation()">Xem chi tiết</a>
            <div class="menu-quantity-control" onclick="event.stopPropagation()">
              <button type="button" ${item.isAvailable === false ? 'disabled' : ''} onclick="event.stopPropagation(); changeMenuQuantity(${item.id}, -1)">-</button>
              <input id="menuQty-${item.id}" type="number" min="1" max="${MENU_MAX_QUANTITY}" value="1" ${item.isAvailable === false ? 'disabled' : ''} onclick="event.stopPropagation()" oninput="normalizeMenuQuantity(this)" />
              <button type="button" ${item.isAvailable === false ? 'disabled' : ''} onclick="event.stopPropagation(); changeMenuQuantity(${item.id}, 1)">+</button>
            </div>
            <button class="small-btn" type="button" ${item.isAvailable === false ? 'disabled' : ''} onclick="event.stopPropagation(); menuAddToCart(getMenuSource().find(food => food.id === ${item.id}), getMenuQuantity(${item.id}))">Thêm vào giỏ hàng</button>
          </div>
        </div>
      </div>
    </article>
  `;
}

function renderDedicatedMenu() {
  const grid = document.getElementById('dedicatedMenuGrid');
  const resultText = document.getElementById('menuResultText');
  const totalCount = document.getElementById('menuTotalCount');
  const emptyState = document.getElementById('menuEmptyState');
  if (!grid) return;

  renderMenuCategoryNav();
  const items = getFilteredMenuItems();
  grid.innerHTML = items.length ? items.map(menuCardTemplate).join('') : '';

  if (resultText) {
    const categoryText = activeMenuCategory === 'all' ? 'tất cả danh mục' : activeMenuCategory;
    const subText = activeMenuSubcategory === 'all' ? '' : ` - ${activeMenuSubcategory}`;
    const keywordText = activeMenuKeyword ? `, từ khóa "${activeMenuKeyword}"` : '';
    resultText.textContent = `Đang hiển thị ${items.length} món trong ${categoryText}${subText}${keywordText}.`;
  }

  if (totalCount) totalCount.textContent = items.length;
  if (emptyState) emptyState.hidden = items.length > 0;
}

function openMenuDetail(id) {
  const item = getMenuSource().find(food => food.id === id);
  const modal = document.getElementById('menuDetailModal');
  if (!item || !modal) return;

  document.getElementById('menuDetailImage').src = getMenuItemImage(item);
  document.getElementById('menuDetailImage').alt = item.name;
  document.getElementById('menuDetailCategory').textContent = item.category;
  document.getElementById('menuDetailSubcategory').textContent = item.subcategory;
  document.getElementById('menuDetailName').textContent = item.name;
  document.getElementById('menuDetailPrice').textContent = formatMenuPrice(item.price);
  document.getElementById('menuDetailDescription').textContent = item.description;
  document.getElementById('menuDetailStory').textContent = item.story;
  document.getElementById('menuDetailIngredients').innerHTML = item.ingredients.map(ingredient => `<li>${escapeHtml(ingredient)}</li>`).join('');
  document.getElementById('menuDetailAllergy').textContent = item.allergy;

  const action = document.getElementById('menuDetailAction');
  action.onclick = () => {
    if (typeof addToCart === 'function') {
      addToCart(item.name, item.price, getMenuItemImage(item), item.id);
      closeMenuDetail();
      return;
    }

    alert(`Đã chọn ${item.name}. Nhân viên FoodieTTTM sẽ hỗ trợ bạn đặt món.`);
  };

  modal.classList.add('show');
  modal.setAttribute('aria-hidden', 'false');
  document.body.classList.add('modal-open');
}

function closeMenuDetail() {
  const modal = document.getElementById('menuDetailModal');
  if (!modal) return;
  modal.classList.remove('show');
  modal.setAttribute('aria-hidden', 'true');
  document.body.classList.remove('modal-open');
}

document.addEventListener('DOMContentLoaded', async () => {
  await loadMenuItemsFromApi();

  document.querySelectorAll('.menu-filter-buttons button').forEach(button => {
    button.addEventListener('click', () => {
      document.querySelectorAll('.menu-filter-buttons button').forEach(item => item.classList.remove('active'));
      button.classList.add('active');
      activeMenuCategory = button.dataset.category || 'all';
      activeMenuSubcategory = 'all';
      renderDedicatedMenu();
    });
  });

  document.getElementById('menuQuickSearch')?.addEventListener('input', event => {
    activeMenuKeyword = event.target.value.trim();
    renderDedicatedMenu();
  });

  renderDedicatedMenu();
});

document.addEventListener('keydown', event => {
  if (event.key === 'Escape') {
    closeMenuDetail();
  }

  if ((event.key === 'Enter' || event.key === ' ') && event.target?.classList?.contains('menu-card')) {
    event.preventDefault();
    openMenuCardDetail(event.target);
  }
});

