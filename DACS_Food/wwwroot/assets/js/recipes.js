const RECIPES_PER_PAGE = 4;
const TOTAL_RECIPE_PAGES = 4;

const RECIPES = [
  {
    id: 1,
    name: 'Trứng chiên hành',
    category: 'mon-nhanh',
    categoryLabel: 'Món nhanh',
    image: '/assets/images/recipes/mon-man/trung-chien-hanh.jpg',
    difficulty: 'Dễ',
    prepTime: '5 phút',
    cookTime: '7 phút',
    servings: '2 người',
    shortDesc: 'Món trứng chiên đơn giản, dễ làm, phù hợp bữa cơm nhanh.',
    ingredients: ['3 quả trứng gà', '2 nhánh hành lá', '1/2 muỗng cà phê nước mắm', 'Một ít tiêu', 'Dầu ăn'],
    steps: ['Rửa sạch hành lá, cắt nhỏ.', 'Đập trứng ra tô, cho hành lá, nước mắm và tiêu rồi đánh đều.', 'Làm nóng chảo với một ít dầu.', 'Đổ trứng vào chiên lửa vừa đến khi vàng hai mặt.', 'Lấy ra đĩa và dùng khi còn nóng.'],
    tips: 'Không chiên lửa quá lớn vì trứng dễ cháy bên ngoài nhưng bên trong chưa chín đều.',
    safetyNote: 'Rửa tay trước khi nấu. Trứng cần được nấu chín hoàn toàn. Không dùng trứng có mùi lạ hoặc vỏ bị nứt bẩn.',
    allergyNote: 'Món có trứng. Nếu dị ứng trứng, nên chọn công thức khác.'
  },
  {
    id: 2,
    name: 'Canh rau ngót thịt bằm',
    category: 'mon-man',
    categoryLabel: 'Món mặn',
    image: '/assets/images/recipes/mon-man/canh-rau-ngot-thit-bam.jpg',
    difficulty: 'Dễ',
    prepTime: '10 phút',
    cookTime: '15 phút',
    servings: '3 người',
    shortDesc: 'Canh rau ngót thịt bằm thanh nhẹ, hợp bữa cơm gia đình.',
    ingredients: ['1 bó rau ngót', '150g thịt heo bằm', '1 củ hành tím', 'Muối', 'Nước mắm', 'Tiêu'],
    steps: ['Rửa tay, nhặt rau ngót và rửa nhiều lần dưới nước sạch.', 'Ướp thịt bằm với hành tím, ít nước mắm và tiêu.', 'Đun nước sôi, cho thịt vào khuấy tơi.', 'Cho rau ngót vào, nấu đến khi rau mềm và thịt chín kỹ.', 'Nêm lại vừa ăn rồi tắt bếp.'],
    tips: 'Vò nhẹ rau ngót trước khi nấu để rau mềm và nước canh ngọt hơn.',
    safetyNote: 'Thịt bằm phải được nấu chín kỹ. Không dùng thịt có màu hoặc mùi lạ.',
    allergyNote: 'Món thường không có nhóm dị ứng phổ biến, nhưng có thể chứa nước mắm.'
  },
  {
    id: 3,
    name: 'Cơm chiên trứng',
    category: 'mon-nhanh',
    categoryLabel: 'Món nhanh',
    image: '/assets/images/recipes/mon-man/com-chien-trung.jpg',
    difficulty: 'Dễ',
    prepTime: '8 phút',
    cookTime: '12 phút',
    servings: '2 người',
    shortDesc: 'Tận dụng cơm nguội, trứng và rau củ để có bữa nhanh gọn.',
    ingredients: ['2 chén cơm nguội', '2 quả trứng', '1/2 củ cà rốt', 'Hành lá', 'Dầu ăn', 'Nước mắm', 'Tiêu'],
    steps: ['Rửa sạch cà rốt, hành lá rồi cắt nhỏ.', 'Đánh tan trứng.', 'Làm nóng chảo, cho trứng vào đảo tơi.', 'Thêm cơm nguội và cà rốt, đảo đều đến khi cơm săn.', 'Nêm nước mắm, tiêu, thêm hành lá rồi tắt bếp.'],
    tips: 'Dùng cơm nguội để hạt cơm tơi hơn, tránh cơm quá nhão.',
    safetyNote: 'Trứng cần chín hoàn toàn. Bảo quản cơm nguội đúng cách và không dùng cơm có mùi chua.',
    allergyNote: 'Món có trứng và có thể chứa gluten nếu dùng nước tương thay nước mắm.'
  },
  {
    id: 4,
    name: 'Gà kho gừng',
    category: 'mon-man',
    categoryLabel: 'Món mặn',
    image: '/assets/images/recipes/mon-man/ga-kho-gung.jpg',
    difficulty: 'Trung bình',
    prepTime: '15 phút',
    cookTime: '25 phút',
    servings: '3 người',
    shortDesc: 'Gà kho gừng thơm ấm, hợp ăn cùng cơm nóng.',
    ingredients: ['500g thịt gà', '1 củ gừng nhỏ', 'Hành tím', 'Nước mắm', 'Đường', 'Tiêu', 'Dầu ăn'],
    steps: ['Rửa tay, rửa thịt gà với nước sạch và để ráo.', 'Cắt gừng sợi, băm hành tím.', 'Ướp gà với nước mắm, đường, tiêu và hành tím 15 phút.', 'Phi thơm gừng, cho gà vào đảo săn.', 'Thêm ít nước, kho lửa nhỏ đến khi gà chín kỹ và nước kho sệt lại.'],
    tips: 'Không kho lửa quá lớn vì gà dễ khô bên ngoài mà chưa thấm đều.',
    safetyNote: 'Thịt gà phải chín kỹ, không còn màu hồng ở phần sát xương. Dùng dao/thớt riêng cho thịt sống nếu có thể.',
    allergyNote: 'Món có thể chứa nước mắm. Người dị ứng cá nên cân nhắc.'
  },
  {
    id: 5,
    name: 'Đậu hũ sốt cà chua',
    category: 'mon-chay',
    categoryLabel: 'Món chay',
    image: '/assets/images/recipes/mon-chay/dau-hu-sot-ca-chua.jpg',
    difficulty: 'Dễ',
    prepTime: '8 phút',
    cookTime: '15 phút',
    servings: '2 người',
    shortDesc: 'Đậu hũ mềm sốt cà chua, dễ ăn cùng cơm nóng.',
    ingredients: ['2 miếng đậu hũ', '2 quả cà chua', 'Hành lá', 'Dầu ăn', 'Muối', 'Tiêu'],
    steps: ['Rửa sạch cà chua, hành lá.', 'Cắt đậu hũ miếng vừa ăn, áp chảo nhẹ nếu thích.', 'Xào cà chua đến khi mềm thành sốt.', 'Cho đậu hũ vào, nêm muối và tiêu.', 'Đun nhỏ lửa vài phút để đậu thấm sốt.'],
    tips: 'Nên đảo nhẹ tay để đậu hũ không bị nát.',
    safetyNote: 'Không dùng đậu hũ có mùi chua hoặc nhớt. Rau củ cần rửa sạch trước khi cắt.',
    allergyNote: 'Món có đậu nành. Nếu dị ứng đậu nành, nên chọn công thức khác.'
  },
  {
    id: 6,
    name: 'Rau muống xào tỏi',
    category: 'mon-chay',
    categoryLabel: 'Món chay',
    image: '/assets/images/recipes/mon-chay/rau-muong-xao-toi.jpg',
    difficulty: 'Dễ',
    prepTime: '10 phút',
    cookTime: '7 phút',
    servings: '3 người',
    shortDesc: 'Rau muống xanh giòn, xào nhanh với tỏi thơm.',
    ingredients: ['1 bó rau muống', '4 tép tỏi', 'Dầu ăn', 'Muối', 'Hạt nêm chay'],
    steps: ['Nhặt rau muống, rửa sạch nhiều lần và để ráo.', 'Đập dập tỏi.', 'Làm nóng chảo với dầu, phi thơm tỏi.', 'Cho rau vào xào lửa lớn, đảo nhanh.', 'Nêm vừa ăn và tắt bếp khi rau vừa chín.'],
    tips: 'Xào lửa lớn và nhanh để rau giữ màu xanh.',
    safetyNote: 'Rau cần rửa kỹ để loại bỏ đất cát. Không dùng rau bị úa, nhớt hoặc có mùi lạ.',
    allergyNote: 'Món thường không có nhóm dị ứng phổ biến.'
  },
  {
    id: 7,
    name: 'Canh bí đỏ thịt bằm',
    category: 'mon-man',
    categoryLabel: 'Món mặn',
    image: '/assets/images/recipes/mon-man/canh-bi-do-thit-bam.jpg',
    difficulty: 'Dễ',
    prepTime: '12 phút',
    cookTime: '18 phút',
    servings: '3 người',
    shortDesc: 'Canh bí đỏ ngọt nhẹ, thịt bằm mềm, hợp bữa cơm nhà.',
    ingredients: ['300g bí đỏ', '120g thịt bằm', 'Hành lá', 'Muối', 'Nước mắm', 'Tiêu'],
    steps: ['Rửa tay, gọt bí đỏ và rửa sạch trước khi cắt miếng.', 'Ướp thịt bằm với ít nước mắm và tiêu.', 'Đun nước sôi, cho thịt vào khuấy tơi.', 'Cho bí đỏ vào nấu đến khi mềm.', 'Nêm vừa ăn, thêm hành lá rồi tắt bếp.'],
    tips: 'Không cắt bí quá nhỏ vì bí dễ nát khi nấu.',
    safetyNote: 'Thịt bằm cần nấu chín kỹ. Không dùng bí bị mốc hoặc mềm nhũn bất thường.',
    allergyNote: 'Món có thể chứa nước mắm.'
  },
  {
    id: 8,
    name: 'Mì xào rau củ',
    category: 'mon-nhanh',
    categoryLabel: 'Món nhanh',
    image: '/assets/images/recipes/mon-chay/mi-xao-rau-cu.jpg',
    difficulty: 'Dễ',
    prepTime: '10 phút',
    cookTime: '10 phút',
    servings: '2 người',
    shortDesc: 'Mì xào nhanh với rau củ, phù hợp bữa tối đơn giản.',
    ingredients: ['2 vắt mì', 'Cải thìa', 'Cà rốt', 'Nấm', 'Tỏi', 'Nước tương', 'Dầu ăn'],
    steps: ['Rửa sạch rau củ và nấm, cắt vừa ăn.', 'Trụng mì vừa mềm rồi để ráo.', 'Phi thơm tỏi, cho rau củ vào xào.', 'Thêm mì và nước tương, đảo đều.', 'Tắt bếp khi rau vừa chín và mì thấm vị.'],
    tips: 'Không trụng mì quá lâu để mì không bị bở khi xào.',
    safetyNote: 'Rau củ phải rửa sạch. Nếu dùng nấm, không dùng nấm có mùi lạ hoặc bị nhớt.',
    allergyNote: 'Món có thể chứa gluten và đậu nành từ mì hoặc nước tương.'
  },
  {
    id: 9,
    name: 'Salad ức gà',
    category: 'healthy',
    categoryLabel: 'Healthy',
    image: '/assets/images/recipes/healthy/salad-uc-ga.jpg',
    difficulty: 'Dễ',
    prepTime: '12 phút',
    cookTime: '12 phút',
    servings: '2 người',
    shortDesc: 'Salad rau xanh với ức gà áp chảo, nhẹ bụng và đủ protein.',
    ingredients: ['200g ức gà', 'Xà lách', 'Cà chua bi', 'Dưa leo', 'Sốt mè rang hoặc dầu olive', 'Tiêu'],
    steps: ['Rửa tay, rửa rau kỹ và để ráo.', 'Ướp ức gà với ít muối, tiêu.', 'Áp chảo ức gà đến khi chín hoàn toàn.', 'Cắt gà thành lát mỏng.', 'Trộn rau, gà và sốt ngay trước khi ăn.'],
    tips: 'Để gà nghỉ vài phút sau khi áp chảo rồi mới cắt để thịt mềm hơn.',
    safetyNote: 'Gà phải chín kỹ, không còn màu hồng bên trong. Dao/thớt cắt gà sống nên tách riêng với rau ăn sống.',
    allergyNote: 'Sốt mè có thể chứa mè, đậu nành hoặc gluten tùy loại.'
  },
  {
    id: 10,
    name: 'Cơm gạo lứt ức gà',
    category: 'healthy',
    categoryLabel: 'Healthy',
    image: '/assets/images/recipes/healthy/com-gao-lut-uc-ga.jpg',
    difficulty: 'Trung bình',
    prepTime: '15 phút',
    cookTime: '30 phút',
    servings: '2 người',
    shortDesc: 'Bữa healthy gồm gạo lứt, ức gà và rau củ luộc.',
    ingredients: ['1 chén gạo lứt', '250g ức gà', 'Bông cải', 'Cà rốt', 'Dầu olive', 'Muối', 'Tiêu'],
    steps: ['Vo gạo lứt và nấu chín.', 'Rửa rau củ, cắt vừa ăn.', 'Ướp ức gà với muối, tiêu.', 'Áp chảo hoặc luộc ức gà đến khi chín kỹ.', 'Luộc rau củ vừa chín, bày cùng cơm và gà.'],
    tips: 'Ngâm gạo lứt 30 phút trước khi nấu để cơm mềm hơn.',
    safetyNote: 'Rửa sạch rau củ. Gà phải chín hoàn toàn trước khi ăn.',
    allergyNote: 'Món thường không có nhóm dị ứng phổ biến, trừ khi dùng sốt ăn kèm có sữa hoặc đậu nành.'
  },
  {
    id: 11,
    name: 'Yến mạch trái cây',
    category: 'healthy',
    categoryLabel: 'Healthy',
    image: '/assets/images/recipes/healthy/yen-mach-trai-cay.jpg',
    difficulty: 'Dễ',
    prepTime: '10 phút',
    cookTime: '0 phút',
    servings: '1 người',
    shortDesc: 'Bữa sáng nhanh với yến mạch, sữa chua và trái cây.',
    ingredients: ['40g yến mạch', '1 hũ sữa chua', '1/2 quả chuối', 'Dâu hoặc táo', 'Hạt dinh dưỡng tùy chọn'],
    steps: ['Rửa tay, rửa sạch trái cây.', 'Cắt trái cây thành miếng vừa ăn.', 'Cho yến mạch vào tô.', 'Thêm sữa chua và trái cây.', 'Trộn đều, dùng ngay hoặc để mát 10 phút.'],
    tips: 'Có thể ngâm yến mạch qua đêm với sữa để mềm hơn.',
    safetyNote: 'Rửa kỹ trái cây ăn sống. Không dùng sữa chua hết hạn hoặc có mùi lạ.',
    allergyNote: 'Món có thể chứa sữa, gluten hoặc hạt. Người dị ứng cần kiểm tra nhãn nguyên liệu.'
  },
  {
    id: 12,
    name: 'Soup bí đỏ',
    category: 'healthy',
    categoryLabel: 'Healthy',
    image: '/assets/images/recipes/healthy/soup-bi-do.jpg',
    difficulty: 'Dễ',
    prepTime: '12 phút',
    cookTime: '20 phút',
    servings: '2 người',
    shortDesc: 'Soup bí đỏ mịn, ấm bụng, hợp bữa sáng hoặc tối nhẹ.',
    ingredients: ['300g bí đỏ', '1/2 củ hành tây', '150ml sữa tươi', 'Nước dùng hoặc nước lọc', 'Muối', 'Tiêu'],
    steps: ['Rửa sạch bí đỏ, gọt vỏ và cắt miếng.', 'Xào hành tây với ít dầu đến thơm.', 'Cho bí đỏ và nước vào nấu mềm.', 'Xay mịn hỗn hợp.', 'Thêm sữa, nêm muối tiêu và đun nhỏ lửa thêm vài phút.'],
    tips: 'Thêm sữa sau khi bí đã mềm để soup không bị tách vị.',
    safetyNote: 'Không dùng bí bị mốc. Khi xay soup nóng, thao tác cẩn thận để tránh bỏng.',
    allergyNote: 'Món có sữa. Có thể thay bằng sữa hạt nếu dị ứng sữa bò.'
  },
  {
    id: 13,
    name: 'Trà chanh mật ong',
    category: 'do-uong',
    categoryLabel: 'Đồ uống',
    image: '/assets/images/recipes/do-uong/tra-chanh-mat-ong.jpg',
    difficulty: 'Dễ',
    prepTime: '5 phút',
    cookTime: '5 phút',
    servings: '1 người',
    shortDesc: 'Trà chanh mật ong thanh nhẹ, dễ pha tại nhà.',
    ingredients: ['1 túi trà hoặc 5g trà', '1/2 quả chanh', '1 muỗng mật ong', '200ml nước nóng', 'Đá tùy thích'],
    steps: ['Rửa tay và rửa sạch chanh.', 'Hãm trà với nước nóng 3-5 phút.', 'Để trà bớt nóng rồi thêm mật ong.', 'Vắt chanh vào và khuấy đều.', 'Thêm đá nếu muốn uống lạnh.'],
    tips: 'Không cho mật ong vào nước quá sôi để giữ hương vị tốt hơn.',
    safetyNote: 'Dùng chanh tươi, không dùng quả mốc hoặc có mùi lạ. Vệ sinh ly, muỗng trước khi pha.',
    allergyNote: 'Mật ong không phù hợp cho trẻ dưới 1 tuổi.'
  },
  {
    id: 14,
    name: 'Nước ép cam',
    category: 'do-uong',
    categoryLabel: 'Đồ uống',
    image: '/assets/images/recipes/do-uong/nuoc-ep-cam.jpg',
    difficulty: 'Dễ',
    prepTime: '7 phút',
    cookTime: '0 phút',
    servings: '1 người',
    shortDesc: 'Nước cam ép tươi, vị chua ngọt tự nhiên.',
    ingredients: ['2 quả cam tươi', 'Đá viên', 'Đường hoặc mật ong tùy chọn'],
    steps: ['Rửa tay và rửa sạch vỏ cam.', 'Cắt đôi cam bằng dao sạch.', 'Vắt hoặc ép lấy nước.', 'Nếm thử, thêm ít đường nếu cần.', 'Dùng ngay để giữ vị tươi.'],
    tips: 'Chọn cam chắc tay, vỏ không mốc để nước ép thơm hơn.',
    safetyNote: 'Rửa sạch cam trước khi cắt để tránh bụi bẩn từ vỏ đi vào nước ép.',
    allergyNote: 'Người nhạy cảm với cam/quýt nên dùng lượng nhỏ trước.'
  },
  {
    id: 15,
    name: 'Sinh tố bơ',
    category: 'do-uong',
    categoryLabel: 'Đồ uống',
    image: '/assets/images/recipes/do-uong/sinh-to-bo.jpg',
    difficulty: 'Dễ',
    prepTime: '8 phút',
    cookTime: '0 phút',
    servings: '1 người',
    shortDesc: 'Sinh tố bơ béo nhẹ, xay mịn và dễ uống.',
    ingredients: ['1 quả bơ chín', '120ml sữa tươi', '1-2 muỗng sữa đặc', 'Đá viên'],
    steps: ['Rửa tay, rửa vỏ bơ trước khi cắt.', 'Bổ bơ, bỏ hạt và lấy phần thịt.', 'Cho bơ, sữa tươi, sữa đặc và đá vào máy xay.', 'Xay mịn.', 'Rót ra ly và dùng ngay.'],
    tips: 'Dùng bơ chín vừa, không bị xơ đen để sinh tố thơm và mịn.',
    safetyNote: 'Không dùng bơ có mùi chua, thâm đen bất thường hoặc bị mốc.',
    allergyNote: 'Món có sữa. Có thể thay bằng sữa hạt nếu dị ứng sữa bò.'
  },
  {
    id: 16,
    name: 'Sữa đậu nành',
    category: 'do-uong',
    categoryLabel: 'Đồ uống',
    image: '/assets/images/recipes/do-uong/sua-dau-nanh.jpg',
    difficulty: 'Trung bình',
    prepTime: '8 giờ ngâm đậu',
    cookTime: '25 phút',
    servings: '4 người',
    shortDesc: 'Sữa đậu nành tự nấu, thơm nhẹ, dùng nóng hoặc lạnh.',
    ingredients: ['200g đậu nành', '1.5 lít nước', 'Đường tùy khẩu vị', 'Lá dứa tùy chọn'],
    steps: ['Nhặt bỏ hạt đậu hư, rửa sạch và ngâm 6-8 giờ.', 'Rửa lại đậu sau khi ngâm.', 'Xay đậu với nước rồi lọc qua khăn sạch.', 'Đun sữa trên lửa vừa, khuấy thường xuyên.', 'Nấu sôi kỹ 10-15 phút, thêm đường nếu thích.'],
    tips: 'Khuấy đều khi nấu để sữa không khét đáy nồi.',
    safetyNote: 'Sữa đậu nành phải được nấu sôi kỹ trước khi uống. Không dùng đậu mốc hoặc có mùi lạ.',
    allergyNote: 'Món có đậu nành. Nếu dị ứng đậu nành, nên chọn đồ uống khác.'
  }
];

let activeRecipeCategory = 'all';
let currentRecipePage = 1;
let apiRecipes = [];
let apiRecipesLoaded = false;

function getRecipeSource() {
  return apiRecipesLoaded && apiRecipes.length ? apiRecipes : RECIPES;
}

function getFilteredRecipes() {
  return getRecipeSource().filter(recipe => activeRecipeCategory === 'all' || recipe.category === activeRecipeCategory);
}

function getPagedRecipes() {
  const start = (currentRecipePage - 1) * RECIPES_PER_PAGE;
  return getFilteredRecipes().slice(start, start + RECIPES_PER_PAGE);
}

function recipeCardTemplate(recipe) {
  return `
    <article class="recipe-card">
      <button type="button" class="recipe-card-button" onclick="openRecipeModal(${recipe.id})">
        <div class="recipe-card-image">
          <img src="${escapeHtml(recipe.image)}" alt="${escapeHtml(recipe.name)}" />
          <span>${escapeHtml(recipe.categoryLabel)}</span>
        </div>
        <div class="recipe-card-body">
          <h3>${escapeHtml(recipe.name)}</h3>
          <p>${escapeHtml(recipe.shortDesc)}</p>
          <div class="recipe-card-meta">
            <span>${escapeHtml(recipe.cookTime)}</span>
            <span>${escapeHtml(recipe.difficulty)}</span>
          </div>
          <strong>Xem công thức</strong>
        </div>
      </button>
    </article>
  `;
}

function renderRecipes() {
  const grid = document.getElementById('recipesGrid');
  const resultText = document.getElementById('recipeResultText');
  if (!grid) return;

  const filtered = getFilteredRecipes();
  const maxPage = Math.max(1, Math.ceil(filtered.length / RECIPES_PER_PAGE));
  if (currentRecipePage > maxPage) currentRecipePage = maxPage;

  const pageItems = getPagedRecipes();
  grid.innerHTML = pageItems.length
    ? pageItems.map(recipeCardTemplate).join('')
    : '<p class="recipe-empty">Chưa có công thức trong trang này.</p>';

  if (resultText) {
    const label = activeRecipeCategory === 'all'
      ? 'tất cả danh mục'
      : document.querySelector(`.recipe-filter-buttons [data-category="${activeRecipeCategory}"]`)?.textContent || activeRecipeCategory;
    resultText.textContent = `Đang hiển thị ${pageItems.length}/${filtered.length} công thức thuộc ${label}, trang ${currentRecipePage}/4.`;
  }

  updateRecipePagination(maxPage);
}

function updateRecipePagination(maxPage) {
  document.querySelectorAll('.recipe-pagination [data-page]').forEach(button => {
    const page = Number(button.dataset.page);
    button.classList.toggle('active', page === currentRecipePage);
    button.disabled = page > maxPage;
  });

  const prev = document.getElementById('recipePrevBtn');
  const next = document.getElementById('recipeNextBtn');
  if (prev) prev.disabled = currentRecipePage <= 1;
  if (next) next.disabled = currentRecipePage >= Math.min(TOTAL_RECIPE_PAGES, maxPage);
}

function openRecipeModal(id) {
  const recipe = getRecipeSource().find(item => item.id === id);
  const modal = document.getElementById('recipeModal');
  if (!recipe || !modal) return;

  document.getElementById('recipeModalImage').src = recipe.image;
  document.getElementById('recipeModalImage').alt = recipe.name;
  document.getElementById('recipeModalCategory').textContent = recipe.categoryLabel;
  document.getElementById('recipeModalDifficulty').textContent = recipe.difficulty;
  document.getElementById('recipeModalName').textContent = recipe.name;
  document.getElementById('recipeModalDesc').textContent = recipe.shortDesc;
  document.getElementById('recipeModalPrep').textContent = `Chuẩn bị: ${recipe.prepTime}`;
  document.getElementById('recipeModalCook').textContent = `Nấu: ${recipe.cookTime}`;
  document.getElementById('recipeModalServings').textContent = `Khẩu phần: ${recipe.servings}`;
  document.getElementById('recipeModalIngredients').innerHTML = recipe.ingredients.map(item => `<li>${escapeHtml(item)}</li>`).join('');
  document.getElementById('recipeModalSteps').innerHTML = recipe.steps.map(item => `<li>${escapeHtml(item)}</li>`).join('');
  document.getElementById('recipeModalTips').textContent = recipe.tips;
  document.getElementById('recipeModalSafety').textContent = recipe.safetyNote;
  document.getElementById('recipeModalAllergy').textContent = recipe.allergyNote;

  modal.classList.add('show');
  modal.setAttribute('aria-hidden', 'false');
  document.body.classList.add('modal-open');
}

function closeRecipeModal() {
  const modal = document.getElementById('recipeModal');
  if (!modal) return;
  modal.classList.remove('show');
  modal.setAttribute('aria-hidden', 'true');
  document.body.classList.remove('modal-open');
}

function normalizeRecipeFromApi(item) {
  return {
    id: item.id,
    name: item.name || item.title,
    category: item.category || 'mon-nhanh',
    categoryLabel: item.categoryLabel || 'Món nhanh',
    image: item.image || item.imageUrl || '/assets/images/recipes/placeholder.jpg',
    difficulty: item.difficulty || 'Dễ',
    prepTime: item.prepTime || '',
    cookTime: item.cookTime || '',
    servings: item.servings || '',
    shortDesc: item.shortDesc || item.description || '',
    ingredients: Array.isArray(item.ingredients) ? item.ingredients : [],
    steps: Array.isArray(item.steps) ? item.steps : [],
    tips: item.tips || '',
    safetyNote: item.safetyNote || '',
    allergyNote: item.allergyNote || ''
  };
}

async function loadRecipesFromApi() {
  try {
    const response = await fetch('/api/recipes?page=1&pageSize=100', {
      headers: { Accept: 'application/json' }
    });

    if (!response.ok) return;

    const data = await response.json();
    const items = Array.isArray(data.items) ? data.items : [];
    if (!items.length) return;

    apiRecipes = items.map(normalizeRecipeFromApi);
    apiRecipesLoaded = true;
  } catch (error) {
    console.warn('Khong the tai cong thuc tu API, dung du lieu tam.', error);
  }
}

document.addEventListener('DOMContentLoaded', async () => {
  await loadRecipesFromApi();

  document.querySelectorAll('.recipe-filter-buttons button').forEach(button => {
    button.addEventListener('click', () => {
      document.querySelectorAll('.recipe-filter-buttons button').forEach(item => item.classList.remove('active'));
      button.classList.add('active');
      activeRecipeCategory = button.dataset.category || 'all';
      currentRecipePage = 1;
      renderRecipes();
    });
  });

  document.querySelectorAll('.recipe-pagination [data-page]').forEach(button => {
    button.addEventListener('click', () => {
      currentRecipePage = Number(button.dataset.page) || 1;
      renderRecipes();
    });
  });

  document.getElementById('recipePrevBtn')?.addEventListener('click', () => {
    currentRecipePage = Math.max(1, currentRecipePage - 1);
    renderRecipes();
  });

  document.getElementById('recipeNextBtn')?.addEventListener('click', () => {
    currentRecipePage = Math.min(TOTAL_RECIPE_PAGES, currentRecipePage + 1);
    renderRecipes();
  });

  renderRecipes();
});

document.addEventListener('keydown', event => {
  if (event.key === 'Escape') {
    closeRecipeModal();
  }
});
