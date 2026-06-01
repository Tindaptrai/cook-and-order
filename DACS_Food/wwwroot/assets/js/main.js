
let currentCategory = 'all';
let cart = JSON.parse(localStorage.getItem('foodie_cart') || '[]');

function formatVND(number) {
  return Number(number).toLocaleString('vi-VN') + 'đ';
}

function saveCart() {
  localStorage.setItem('foodie_cart', JSON.stringify(cart));
}

function clampCartQuantity(value) {
  const quantity = Number.parseInt(value, 10);
  if (Number.isNaN(quantity)) return 1;
  return Math.min(Math.max(quantity, 1), 20);
}

function changeQuantityInput(inputId, delta) {
  const input = document.getElementById(inputId);
  if (!input) return;
  input.value = clampCartQuantity(Number(input.value || 1) + delta);
}

function normalizeQuantityInput(input) {
  if (!input) return;
  input.value = clampCartQuantity(input.value);
}

function mapApiCartItems(data) {
  const items = Array.isArray(data.items) ? data.items : [];
  return items.map(item => ({
    cartItemId: item.cartItemId,
    id: item.foodItemId,
    name: item.name,
    price: item.price,
    image: item.image,
    quantity: item.quantity
  }));
}

async function fetchCartFromApi() {
  const response = await fetch('/api/cart');
  if (!response.ok) throw new Error('Cannot load cart');
  return response.json();
}

async function postCartItemToApi(foodItemId, quantity = 1) {
  const response = await fetch('/api/cart/items', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ foodItemId, quantity })
  });

  if (!response.ok) throw new Error('Cannot add cart item');
  return response.json();
}

async function updateCartItemInApi(cartItemId, quantity) {
  const response = await fetch(`/api/cart/items/${cartItemId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ quantity })
  });

  if (!response.ok) throw new Error('Cannot update cart item');
  return response.json();
}

async function removeCartItemFromApi(cartItemId) {
  const response = await fetch(`/api/cart/items/${cartItemId}`, {
    method: 'DELETE'
  });

  if (!response.ok) throw new Error('Cannot remove cart item');
  return response.json();
}

function applyApiCart(data) {
  cart = mapApiCartItems(data);
  saveCart();
  renderCart();
  renderCheckout();
}

async function loadCartFromApi() {
  try {
    let data = await fetchCartFromApi();
    let apiItems = mapApiCartItems(data);
    const localItems = Array.isArray(cart) ? cart.filter(item => item.id && item.quantity > 0) : [];

    if (!apiItems.length && localItems.length) {
      for (const item of localItems) {
        await postCartItemToApi(item.id, item.quantity);
      }
      data = await fetchCartFromApi();
      apiItems = mapApiCartItems(data);
    }

    cart = apiItems;
    saveCart();
  } catch {
    cart = JSON.parse(localStorage.getItem('foodie_cart') || '[]');
  }
}

function setCategory(category, button) {
  currentCategory = category;

  document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.classList.remove('active');
  });

  if (button) button.classList.add('active');
  filterFoods();
}

function filterFoods() {
  const searchInput = document.getElementById('searchInput');
  const searchValue = searchInput ? searchInput.value.toLowerCase().trim() : '';
  const cards = document.querySelectorAll('.food-card');

  cards.forEach(card => {
    const category = card.dataset.category || '';
    const name = card.dataset.name || '';

    const matchCategory = currentCategory === 'all' || category.includes(currentCategory.toLowerCase());
    const matchSearch = name.includes(searchValue);

    card.style.display = matchCategory && matchSearch ? 'block' : 'none';
  });
}

async function addToCart(name, price, image = '', id = 0, quantity = 1) {
  const addQuantity = clampCartQuantity(quantity);
  if (id) {
    try {
      const data = await postCartItemToApi(id, addQuantity);
      applyApiCart(data);
      const panel = document.getElementById('cartPanel');
      if (panel) panel.classList.add('show');
      return;
    } catch {
      // Fallback to local cart below.
    }
  }

  const existing = cart.find(item => item.name === name);

  if (existing) {
    existing.quantity = Math.min(20, existing.quantity + addQuantity);
    if (image && !existing.image) existing.image = image;
    if (id && !existing.id) existing.id = id;
  } else {
    cart.push({ id, name, price, image, quantity: addQuantity });
  }

  saveCart();
  renderCart();
  renderCheckout();
  const panel = document.getElementById('cartPanel');
  if (panel) panel.classList.add('show');
}

function addToCartFromQuantity(name, price, image, id, inputId) {
  const input = document.getElementById(inputId);
  addToCart(name, price, image, id, clampCartQuantity(input ? input.value : 1));
}

async function changeCartQuantity(index, delta) {
  if (!cart[index]) return;
  if (cart[index].cartItemId) {
    try {
      const nextQuantity = clampCartQuantity(cart[index].quantity + delta);
      const data = await updateCartItemInApi(cart[index].cartItemId, nextQuantity);
      applyApiCart(data);
      return;
    } catch {
      // Fallback to local cart below.
    }
  }

  cart[index].quantity = clampCartQuantity(cart[index].quantity + delta);
  saveCart();
  renderCart();
  renderCheckout();
}

async function removeCartItem(index) {
  if (!cart[index]) return;
  if (cart[index].cartItemId) {
    try {
      const data = await removeCartItemFromApi(cart[index].cartItemId);
      applyApiCart(data);
      return;
    } catch {
      // Fallback to local cart below.
    }
  }

  cart.splice(index, 1);
  saveCart();
  renderCart();
  renderCheckout();
}

function renderCart() {
  const cartList = document.getElementById('cartList');
  const cartCount = document.getElementById('cartCount');
  const cartTotal = document.getElementById('cartTotal');

  const totalQuantity = cart.reduce((sum, item) => sum + item.quantity, 0);
  const totalPrice = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);

  if (cartCount) cartCount.textContent = totalQuantity > 0 ? totalQuantity : '';
  if (cartTotal) cartTotal.textContent = formatVND(totalPrice);

  if (!cartList) return;

  if (cart.length === 0) {
    cartList.innerHTML = '<p class="muted-text">Giỏ hàng đang trống. Khi thêm món, số lượng sẽ hiển thị cạnh nút Giỏ hàng.</p>';
    return;
  }

  cartList.innerHTML = cart.map((item, index) => `
    <div class="cart-item cart-item-rich">
      <img src="${escapeHtml(item.image || 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=200&q=80')}" alt="${escapeHtml(item.name)}" />
      <div class="cart-item-info">
        <strong>${escapeHtml(item.name)}</strong>
        <span>${formatVND(item.price)} / món</span>
        <div class="cart-qty">
          <button type="button" onclick="changeCartQuantity(${index}, -1)">-</button>
          <b>${item.quantity}</b>
          <button type="button" onclick="changeCartQuantity(${index}, 1)">+</button>
          <button type="button" class="cart-remove" onclick="removeCartItem(${index})">Xóa</button>
        </div>
      </div>
      <strong>${formatVND(item.price * item.quantity)}</strong>
    </div>
  `).join('');
}

function toggleCart() {
  const panel = document.getElementById('cartPanel');
  if (panel) panel.classList.toggle('show');
}

function sendMessage() {
  const input = document.getElementById('chatInput');
  const chatMessages = document.getElementById('chatMessages');

  if (!input || !chatMessages) return;

  const text = input.value.trim();
  if (!text) return;

  chatMessages.innerHTML += `<div class="message user">${text}</div>`;

  let reply = 'Mình gợi ý bạn thử cơm gà sốt tiêu đen nếu muốn no, hoặc cơm chay nấm nếu muốn nhẹ bụng.';
  const lower = text.toLowerCase();

  if (lower.includes('trứng')) {
    reply = 'Bạn có thể làm trứng chiên hành hoặc cơm rang trứng. Nhanh, rẻ và dễ làm.';
  } else if (lower.includes('chay') || lower.includes('rằm')) {
    reply = 'Hôm nay bạn có thể chọn cơm chay nấm áp chảo hoặc canh nấm chay.';
  } else if (lower.includes('mệt') || lower.includes('buồn')) {
    reply = 'Bạn nên chọn món nóng, dễ ăn như bún bò mini hoặc cơm gà sốt tiêu đen nhé.';
  } else if (lower.includes('healthy') || lower.includes('giảm cân')) {
    reply = 'Mình gợi ý salad ức gà mật ong, nhiều rau và ít dầu hơn.';
  }

  setTimeout(() => {
    chatMessages.innerHTML += `<div class="message bot">${reply}</div>`;
    chatMessages.scrollTop = chatMessages.scrollHeight;
  }, 350);

  input.value = '';
  chatMessages.scrollTop = chatMessages.scrollHeight;
}

function renderTables() {
  const tableGrid = document.getElementById('tableGrid');
  if (!tableGrid) return;

  let html = '';

  for (let i = 1; i <= 10; i++) {
    const isPrivate = i <= 2;
    const occupied = i === 2 || i === 5;

    html += `
      <div class="table-card ${occupied ? 'occupied' : ''}" onclick="toggleTable(this)">
        <h3>Bàn ${i}</h3>
        <p>${isPrivate ? 'Private' : 'Bàn thường'}</p>
        <span class="status">${occupied ? 'Có khách' : 'Còn trống'}</span>
      </div>
    `;
  }

  tableGrid.innerHTML = html;
}

function toggleTable(card) {
  card.classList.toggle('occupied');
  const status = card.querySelector('.status');
  status.textContent = card.classList.contains('occupied') ? 'Có khách' : 'Còn trống';
}

function renderCheckout() {
  const orderList = document.getElementById('checkoutList');
  const orderTotal = document.getElementById('checkoutTotal');
  const orderCode = document.getElementById('orderCode');
  const cartJson = document.getElementById('checkoutCartJson');

  if (!orderList || !orderTotal) return;

  const totalPrice = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
  if (cartJson) cartJson.value = JSON.stringify(cart);

  if (cart.length === 0) {
    if (!orderList.children.length) {
      orderList.innerHTML = '<p class="muted-text">Giỏ hàng đang trống.</p>';
    }
  } else {
    orderList.innerHTML = cart.map(item => `
      <div class="cart-item cart-item-rich">
        <img src="${escapeHtml(item.image || 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=200&q=80')}" alt="${escapeHtml(item.name)}" />
        <span>${escapeHtml(item.name)} x${item.quantity}</span>
        <strong>${formatVND(item.price * item.quantity)}</strong>
      </div>
    `).join('');
    orderTotal.textContent = formatVND(totalPrice);
  }

  if (orderCode) orderCode.textContent = 'FD' + Math.floor(100000 + Math.random() * 900000);
}

function parsePaymentMethod(value) {
  return String(value || '').toUpperCase() === 'QR' ? 2 : 1;
}

function parseOrderType(value) {
  const normalized = String(value || '').toLowerCase();
  if (normalized === 'dinein' || normalized === '2') return 2;
  if (normalized === 'takeaway' || normalized === '3') return 3;
  return 1;
}

function showCheckoutMessage(form, message) {
  let element = document.getElementById('checkoutApiMessage');
  if (!element) {
    element = document.createElement('p');
    element.id = 'checkoutApiMessage';
    element.className = 'muted-text';
    form.insertBefore(element, form.querySelector('.form-group'));
  }
  element.textContent = message;
}

function initCheckoutApiForm() {
  const form = document.getElementById('checkoutForm');
  if (!form) return;

  form.addEventListener('submit', async (event) => {
    event.preventDefault();

    const submitButton = form.querySelector('button[type="submit"]');
    if (submitButton) submitButton.disabled = true;
    showCheckoutMessage(form, 'Đang tạo đơn hàng...');

    const formData = new FormData(form);
    const cartJson = document.getElementById('checkoutCartJson');
    if (cartJson) cartJson.value = JSON.stringify(cart);

    const payload = {
      customerName: formData.get('CustomerName') || '',
      phoneNumber: formData.get('PhoneNumber') || '',
      customerEmail: formData.get('CustomerEmail') || '',
      address: formData.get('Address') || '',
      orderType: parseOrderType(formData.get('OrderType')),
      tableId: formData.get('TableId') ? Number(formData.get('TableId')) : null,
      paymentMethod: parsePaymentMethod(formData.get('PaymentMethod')),
      discountCode: formData.get('DiscountCode') || null,
      cartJson: cartJson ? cartJson.value : JSON.stringify(cart)
    };

    try {
      const response = await fetch('/api/orders', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify(payload)
      });
      const data = await response.json().catch(() => ({}));

      if (!response.ok) {
        showCheckoutMessage(form, data.message || 'Không thể tạo đơn hàng. Vui lòng kiểm tra lại thông tin.');
        return;
      }

      localStorage.removeItem('foodie_cart');
      cart = [];
      renderCart();
      window.location.href = data.redirectUrl || `/orders/success/${data.orderCode}`;
    } catch (error) {
      console.warn('Khong the tao don qua API, chuyen sang form fallback.', error);
      form.submit();
    } finally {
      if (submitButton) submitButton.disabled = false;
    }
  });
}

function initDemoPaymentApiForm() {
  const form = document.getElementById('demoPaymentForm');
  if (!form) return;

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const orderCode = new FormData(form).get('orderCode');

    try {
      const response = await fetch('/api/payments/demo-confirm', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json'
        },
        body: JSON.stringify({ orderCode })
      });
      const data = await response.json().catch(() => ({}));

      if (!response.ok) {
        alert(data.message || 'Không thể xác nhận thanh toán.');
        return;
      }

      window.location.href = data.redirectUrl || `/orders/success/${orderCode}`;
    } catch (error) {
      console.warn('Khong the xac nhan thanh toan qua API, chuyen sang form fallback.', error);
      form.submit();
    }
  });
}

document.addEventListener('DOMContentLoaded', async () => {
  await loadCartFromApi();
  renderCart();
  renderTables();
  renderCheckout();
  initCheckoutApiForm();
  initDemoPaymentApiForm();

  const chatInput = document.getElementById('chatInput');
  if (chatInput) {
    chatInput.addEventListener('keydown', function(event) {
      if (event.key === 'Enter') sendMessage();
    });
  }
});


function toggleFloatingChat() {
  const chatWindow = document.getElementById('floatingChatWindow');
  if (!chatWindow) return;
  chatWindow.classList.toggle('show');

  if (chatWindow.classList.contains('show')) {
    setTimeout(() => {
      const input = document.getElementById('floatingChatInput');
      if (input) input.focus();
    }, 120);
  }
}

let foodieBotMessengerUrl = '';
let lastChatbotFoods = [];
let foodieChatHistory = [];

const FOODIE_CHAT_SESSION_KEY = 'foodie_chat_session_id';
const FOODIE_CHAT_WELCOME = 'Xin chào! Mình là FoodieBot. Bạn muốn ăn no, ăn nhẹ, món chay hay món theo giá hôm nay?';

function getFoodieChatSessionId() {
  let sessionId = localStorage.getItem(FOODIE_CHAT_SESSION_KEY);
  if (!sessionId) {
    sessionId = `chat-${Date.now()}-${Math.random().toString(16).slice(2)}`;
    localStorage.setItem(FOODIE_CHAT_SESSION_KEY, sessionId);
  }
  return sessionId;
}

function getFoodieChatHistoryKey() {
  return `foodie_chat_history_guest_${getFoodieChatSessionId()}`;
}

function isFoodieUserLoggedIn() {
  return document.body && document.body.dataset.chatAuthenticated === 'true';
}

function readLocalChatHistory() {
  try {
    return JSON.parse(localStorage.getItem(getFoodieChatHistoryKey()) || '[]');
  } catch {
    return [];
  }
}

function writeLocalChatHistory(history) {
  localStorage.setItem(getFoodieChatHistoryKey(), JSON.stringify(history.slice(-200)));
}

function chatHistoryKey(item) {
  return `${item.role || ''}|${item.message || ''}`;
}

function mergeChatHistory(primary, secondary) {
  const result = [];
  const seen = new Set();
  [...primary, ...secondary].forEach(item => {
    if (!item || !item.message) return;
    const key = chatHistoryKey(item);
    if (seen.has(key)) return;
    seen.add(key);
    result.push(item);
  });
  return result.sort((a, b) => new Date(a.createdAt || 0) - new Date(b.createdAt || 0));
}

function appendChatHistory(role, message, metadata = {}) {
  const item = {
    role,
    message,
    intent: metadata.intent || null,
    createdAt: metadata.createdAt || new Date().toISOString(),
    pageUrl: metadata.pageUrl || window.location.pathname + window.location.search,
    metadataJson: metadata.metadataJson || null
  };
  foodieChatHistory = mergeChatHistory(foodieChatHistory, [item]);
  writeLocalChatHistory(foodieChatHistory);
  return item;
}

function renderFloatingChatHistory() {
  const messages = document.getElementById('floatingChatMessages');
  if (!messages) return;

  if (!foodieChatHistory.length) {
    messages.innerHTML = `<div class="message bot">${escapeHtml(FOODIE_CHAT_WELCOME)}</div>`;
  } else {
    messages.innerHTML = foodieChatHistory.map(item => {
      const roleClass = item.role === 'assistant' ? 'bot' : 'user';
      return `<div class="message ${roleClass}">${escapeHtml(item.message)}</div>`;
    }).join('');
  }

  messages.scrollTop = messages.scrollHeight;
}

async function loadFloatingChatHistory() {
  foodieChatHistory = readLocalChatHistory();
  renderFloatingChatHistory();

  if (!isFoodieUserLoggedIn()) {
    return;
  }

  try {
    const response = await fetch(`/api/chatbot/history?sessionId=${encodeURIComponent(getFoodieChatSessionId())}`);
    if (!response.ok) throw new Error('Cannot load chat history');
    const data = await response.json();
    const apiHistory = Array.isArray(data.messages) ? data.messages : [];
    foodieChatHistory = mergeChatHistory(apiHistory, foodieChatHistory);
    writeLocalChatHistory(foodieChatHistory);
    renderFloatingChatHistory();
  } catch {
    foodieChatHistory = readLocalChatHistory();
    renderFloatingChatHistory();
  }
}

function chatbotFoodCardTemplate(item, index) {
  const price = item.discountPrice || item.price || 0;
  const originalPrice = item.discountPrice
    ? `<span class="chatbot-food-old-price">${formatVND(item.price)}</span>`
    : '';
  const badges = [
    item.isFeatured ? 'Nổi bật' : '',
    item.isVegetarian ? 'Món chay' : '',
    item.isAvailable ? 'Còn món' : 'Hết món'
  ].filter(Boolean);

  return `
    <article class="chatbot-food-card">
      <img src="${escapeHtml(item.imageUrl || '/images/foods/fallback-food.jpg')}" alt="${escapeHtml(item.name)}" onerror="this.src='/images/foods/fallback-food.jpg'" />
      <div class="chatbot-food-info">
        <div class="chatbot-food-head">
          <strong>${escapeHtml(item.name)}</strong>
          <span>${formatVND(price)}${originalPrice}</span>
        </div>
        <p>${escapeHtml(item.description || '')}</p>
        <div class="chatbot-food-badges">
          ${badges.map(badge => `<small>${escapeHtml(badge)}</small>`).join('')}
        </div>
        <div class="chatbot-food-actions">
          <a href="${escapeHtml(item.detailUrl || `/menu/${item.slug}`)}">Xem chi tiết</a>
          <button type="button" ${item.isAvailable ? '' : 'disabled'} onclick="addChatbotFoodToCart(${index})">Thêm vào giỏ</button>
        </div>
      </div>
    </article>
  `;
}

function addChatbotFoodToCart(index) {
  const item = lastChatbotFoods[index];
  if (!item) return;
  addToCart(item.name, item.discountPrice || item.price || 0, item.imageUrl || '', item.id || 0);
}

function renderChatbotResponse(response) {
  const parts = [`<div>${escapeHtml(response.reply || response.message || 'Dạ FoodieBot chưa hiểu rõ, bạn nhập lại giúp mình nha.')}</div>`];

  const foods = Array.isArray(response.suggestedFoods) ? response.suggestedFoods : [];
  lastChatbotFoods = foods;
  if (foods.length) {
    parts.push(`
      <div class="chatbot-result-block">
        <strong>Món gợi ý</strong>
        <div class="chatbot-food-cards">
          ${foods.map(chatbotFoodCardTemplate).join('')}
        </div>
      </div>
    `);
  }

  const recipes = Array.isArray(response.suggestedRecipes) ? response.suggestedRecipes : response.recipeLinks;
  if (Array.isArray(recipes) && recipes.length) {
    parts.push(`
      <div class="chatbot-result-block">
        <strong>Công thức</strong>
        <div class="chatbot-recipe-links">
          ${recipes.map(item => `<a href="${escapeHtml(item.url)}">${escapeHtml(item.title)}</a>`).join('')}
        </div>
      </div>
    `);
  }

  if (response.needHumanSupport && response.messengerUrl) {
    foodieBotMessengerUrl = response.messengerUrl;
    parts.push(`<button type="button" class="chatbot-messenger-inline" onclick="openMessengerSupport()">Chat với nhân viên qua Messenger</button>`);
  }

  return parts.join('');
}

async function askFoodieBot(text) {
  const response = await fetch('/api/chatbot/ask', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      message: text,
      sessionId: getFoodieChatSessionId(),
      pageUrl: window.location.pathname + window.location.search,
      clientHistory: isFoodieUserLoggedIn() ? foodieChatHistory : []
    })
  });

  if (!response.ok) {
    throw new Error('Chatbot request failed');
  }

  return response.json();
}

async function sendFloatingMessage() {
  const input = document.getElementById('floatingChatInput');
  const messages = document.getElementById('floatingChatMessages');
  if (!input || !messages) return;

  const text = input.value.trim();
  if (!text) return;

  messages.innerHTML += `<div class="message user">${escapeHtml(text)}</div>`;
  appendChatHistory('user', text);
  input.value = '';
  messages.scrollTop = messages.scrollHeight;

  const thinking = document.createElement('div');
  thinking.className = 'message bot chatbot-thinking';
  thinking.textContent = 'FoodieLab đang nghĩ món phù hợp...';
  messages.appendChild(thinking);
  messages.scrollTop = messages.scrollHeight;

  try {
    const response = await askFoodieBot(text);
    foodieBotMessengerUrl = response.messengerUrl || foodieBotMessengerUrl;
    thinking.innerHTML = renderChatbotResponse(response);
    appendChatHistory('assistant', response.reply || response.message || 'FoodieBot đã trả lời.', {
      intent: response.intent || null,
      metadataJson: JSON.stringify({
        suggestedFoods: response.suggestedFoods || [],
        suggestedRecipes: response.suggestedRecipes || [],
        needHumanSupport: !!response.needHumanSupport,
        messengerUrl: response.messengerUrl || null
      })
    });
    messages.scrollTop = messages.scrollHeight;
  } catch {
    const fallback = 'Hiện FoodieLab chưa trả lời được ngay. Tin nhắn của bạn đã được giữ lại, bạn thử lại sau ít phút nhé.';
    thinking.innerHTML = escapeHtml(fallback);
    appendChatHistory('assistant', fallback, { intent: 'FoodRecommendation' });
  }
}

function quickChat(text) {
  const chatWindow = document.getElementById('floatingChatWindow');
  if (chatWindow && !chatWindow.classList.contains('show')) {
    chatWindow.classList.add('show');
  }

  const input = document.getElementById('floatingChatInput');
  if (!input) return;

  input.value = text;
  sendFloatingMessage();
}

async function clearFloatingChat() {
  foodieChatHistory = [];
  localStorage.removeItem(getFoodieChatHistoryKey());
  renderFloatingChatHistory();

  try {
    await fetch('/api/chatbot/clear', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ sessionId: getFoodieChatSessionId() })
    });
  } catch {
    // Local history was already cleared; server history will be retried on next explicit clear.
  }
}

document.addEventListener('DOMContentLoaded', () => {
  loadFloatingChatHistory();

  const floatingInput = document.getElementById('floatingChatInput');
  if (floatingInput) {
    floatingInput.addEventListener('keydown', function(event) {
      if (event.key === 'Enter') {
        sendFloatingMessage();
      }
    });
  }
});

function openMessengerSupport(url) {
  const targetUrl = url || foodieBotMessengerUrl || '';
  if (!targetUrl) return;
  window.open(targetUrl, '_blank', 'noopener,noreferrer');
}


function toggleMobileMenu() {
  const panel = document.getElementById('mobileNavPanel');
  if (!panel) return;
  panel.classList.toggle('show');
}

document.addEventListener('click', function(event) {
  const panel = document.getElementById('mobileNavPanel');
  const btn = document.querySelector('.mobile-menu-btn');

  if (!panel || !btn) return;

  const clickedInsidePanel = panel.contains(event.target);
  const clickedButton = btn.contains(event.target);

  if (!clickedInsidePanel && !clickedButton && panel.classList.contains('show')) {
    panel.classList.remove('show');
  }
});

// Upgrade renderCart to also update mobile cart count
const originalRenderCart = renderCart;
renderCart = function() {
  originalRenderCart();

  const mobileCartCount = document.getElementById('mobileCartCount');
  if (mobileCartCount) {
    const totalQuantity = cart.reduce((sum, item) => sum + item.quantity, 0);
    mobileCartCount.textContent = totalQuantity > 0 ? totalQuantity : '';
  }
};


function openFoodDetail(card) {
  const modal = document.getElementById('foodDetailModal');
  if (!modal || !card) return;

  const name = card.querySelector('h3') ? card.querySelector('h3').textContent.trim() : 'Món ăn';
  const desc = card.querySelector('.food-body p') ? card.querySelector('.food-body p').textContent.trim() : '';
  const price = card.querySelector('.price') ? card.querySelector('.price').textContent.trim() : '0đ';
  const image = card.querySelector('.food-img img') ? card.querySelector('.food-img img').src : '';
  const tag = card.querySelector('.food-tag') ? card.querySelector('.food-tag').textContent.trim() : 'Món ngon';
  const ingredients = (card.dataset.ingredients || 'Nguyên liệu chính, Gia vị cơ bản, Rau ăn kèm').split(',').map(x => x.trim()).filter(Boolean);
  const story = card.dataset.story || 'Món ăn này được tạo ra để mang lại trải nghiệm gần gũi, dễ ăn và phù hợp nhiều nhóm khách.';

  document.getElementById('detailName').textContent = name;
  document.getElementById('detailDesc').textContent = desc;
  document.getElementById('detailPrice').textContent = price;
  document.getElementById('detailImage').src = image;
  document.getElementById('detailTag').textContent = tag;
  document.getElementById('detailStory').textContent = story;

  const ingredientList = document.getElementById('detailIngredients');
  ingredientList.innerHTML = ingredients.map(item => `<li>${item}</li>`).join('');

  const rawPrice = Number(price.replace(/[^\d]/g, '')) || 0;
  const addBtn = document.getElementById('detailAddBtn');
  addBtn.onclick = function() {
    addToCart(name, rawPrice, image);
    closeFoodDetail();
  };

  modal.classList.add('show');
  document.body.classList.add('modal-open');
}

function closeFoodDetail() {
  const modal = document.getElementById('foodDetailModal');
  if (!modal) return;
  modal.classList.remove('show');
  document.body.classList.remove('modal-open');
}

document.addEventListener('keydown', function(event) {
  if (event.key === 'Escape') {
    closeFoodDetail();
  }
});


// GOLDEN HOUR SETTINGS
// Khung giờ vàng: 10:30 - 12:30 và 16:00 - 18:00 theo giờ máy người dùng.
const GOLDEN_HOUR_WINDOWS = [
  { startHour: 10, startMinute: 30, endHour: 12, endMinute: 30 },
  { startHour: 16, startMinute: 0, endHour: 18, endMinute: 0 }
];

let goldenHourManuallyClosed = false;

function getTodayTime(hour, minute) {
  const now = new Date();
  const time = new Date(now);
  time.setHours(hour, minute, 0, 0);
  return time;
}

function getCurrentGoldenHourWindow() {
  const now = new Date();

  for (const windowTime of GOLDEN_HOUR_WINDOWS) {
    const start = getTodayTime(windowTime.startHour, windowTime.startMinute);
    const end = getTodayTime(windowTime.endHour, windowTime.endMinute);

    if (now >= start && now < end) {
      return { start, end };
    }
  }

  return null;
}

function formatCountdown(milliseconds) {
  const totalSeconds = Math.max(0, Math.floor(milliseconds / 1000));
  const hours = String(Math.floor(totalSeconds / 3600)).padStart(2, '0');
  const minutes = String(Math.floor((totalSeconds % 3600) / 60)).padStart(2, '0');
  const seconds = String(totalSeconds % 60).padStart(2, '0');

  return `${hours}:${minutes}:${seconds}`;
}

function closeGoldenHourBanner() {
  goldenHourManuallyClosed = true;
  const banner = document.getElementById('goldenHourBanner');
  if (banner) banner.classList.remove('show');
}

function updateGoldenHourBanner() {
  const banner = document.getElementById('goldenHourBanner');
  const countdown = document.getElementById('goldenHourCountdown');

  if (!banner || !countdown) return;

  const activeWindow = getCurrentGoldenHourWindow();

  if (!activeWindow) {
    banner.classList.remove('show');
    goldenHourManuallyClosed = false;
    return;
  }

  const now = new Date();
  const remaining = activeWindow.end - now;

  countdown.textContent = formatCountdown(remaining);

  if (!goldenHourManuallyClosed) {
    banner.classList.add('show');
  }

  if (remaining <= 0) {
    banner.classList.remove('show');
    goldenHourManuallyClosed = false;
  }
}

document.addEventListener('DOMContentLoaded', () => {
  updateGoldenHourBanner();
  setInterval(updateGoldenHourBanner, 1000);
});


const FULL_MENU_ITEMS = [
  {
    "id": 1,
    "name": "Cơm gà sốt tiêu đen",
    "category": "Cơm",
    "price": 59000,
    "tag": "Bán chạy",
    "image": "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm gà sốt tiêu đen được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Thịt gà",
      "Gia vị ướp"
    ],
    "story": "Cơm gà sốt tiêu đen là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 2,
    "name": "Mì Ý bò bằm",
    "category": "Mì",
    "price": 69000,
    "tag": "Giờ vàng",
    "image": "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
    "desc": "Mì Ý bò bằm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị",
      "Sợi mì/bún",
      "Nước dùng"
    ],
    "story": "Mì Ý bò bằm mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 3,
    "name": "Cơm chay nấm áp chảo",
    "category": "Món chay",
    "price": 49000,
    "tag": "Ngày rằm",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm chay nấm áp chảo được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Cơm chay nấm áp chảo được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 4,
    "name": "Salad ức gà mật ong",
    "category": "Healthy",
    "price": 65000,
    "tag": "Healthy",
    "image": "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
    "desc": "Salad ức gà mật ong được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành",
      "Rau xanh",
      "Protein ít béo"
    ],
    "story": "Salad ức gà mật ong là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 5,
    "name": "Bún bò mini",
    "category": "Mì",
    "price": 55000,
    "tag": "No lâu",
    "image": "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80",
    "desc": "Bún bò mini được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị",
      "Sợi mì/bún",
      "Nước dùng"
    ],
    "story": "Bún bò mini mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 6,
    "name": "Cơm chiên hải sản",
    "category": "Cơm",
    "price": 62000,
    "tag": "Món nóng",
    "image": "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm chiên hải sản được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm chiên hải sản là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 7,
    "name": "Gà giòn sốt cay",
    "category": "Gà",
    "price": 72000,
    "tag": "Best seller",
    "image": "https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=900&q=80",
    "desc": "Gà giòn sốt cay được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành"
    ],
    "story": "Gà giòn sốt cay là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 8,
    "name": "Burger bò phô mai",
    "category": "Fastfood",
    "price": 79000,
    "tag": "Đậm vị",
    "image": "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80",
    "desc": "Burger bò phô mai được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị",
      "Vỏ bánh",
      "Nhân chính"
    ],
    "story": "Burger bò phô mai mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 9,
    "name": "Mì cay hải sản",
    "category": "Mì",
    "price": 68000,
    "tag": "Cay nhẹ",
    "image": "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=900&q=80",
    "desc": "Mì cay hải sản được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Sợi mì/bún",
      "Nước dùng",
      "Rau thơm"
    ],
    "story": "Mì cay hải sản gợi cảm giác ấm bụng và quen thuộc. Món phù hợp cho những lúc muốn ăn nhanh nhưng vẫn cần hương vị rõ ràng, có nước dùng hoặc sốt làm điểm nhấn."
  },
  {
    "id": 10,
    "name": "Cơm sườn nướng mật ong",
    "category": "Cơm",
    "price": 64000,
    "tag": "Thơm ngon",
    "image": "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm sườn nướng mật ong được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm sườn nướng mật ong là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 11,
    "name": "Salad cá ngừ",
    "category": "Healthy",
    "price": 61000,
    "tag": "Eat clean",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Salad cá ngừ được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Rau xanh",
      "Protein ít béo",
      "Sốt nhẹ"
    ],
    "story": "Salad cá ngừ dành cho khách muốn ăn nhẹ mà vẫn đủ chất. Câu chuyện của món nằm ở sự cân bằng giữa rau xanh, protein và phần sốt vừa đủ để không làm mất cảm giác tươi."
  },
  {
    "id": 12,
    "name": "Trà đào cam sả",
    "category": "Đồ uống",
    "price": 29000,
    "tag": "Mát lạnh",
    "image": "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
    "desc": "Trà đào cam sả được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Trà đào cam sả được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 13,
    "name": "Cơm bò lúc lắc",
    "category": "Cơm",
    "price": 75000,
    "tag": "Bán chạy",
    "image": "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm bò lúc lắc được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Thịt bò",
      "Hành tây"
    ],
    "story": "Cơm bò lúc lắc mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 14,
    "name": "Phở bò tái",
    "category": "Mì",
    "price": 58000,
    "tag": "Nước dùng ngon",
    "image": "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
    "desc": "Phở bò tái được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị",
      "Sợi mì/bún",
      "Nước dùng"
    ],
    "story": "Phở bò tái mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 15,
    "name": "Bánh mì gà xé",
    "category": "Fastfood",
    "price": 39000,
    "tag": "Nhanh gọn",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Bánh mì gà xé được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành",
      "Sợi mì/bún",
      "Nước dùng"
    ],
    "story": "Bánh mì gà xé là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 16,
    "name": "Cơm chay đậu hũ sốt nấm",
    "category": "Món chay",
    "price": 52000,
    "tag": "Thanh nhẹ",
    "image": "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm chay đậu hũ sốt nấm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Cơm chay đậu hũ sốt nấm được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 17,
    "name": "Nui xào bò",
    "category": "Mì",
    "price": 57000,
    "tag": "Dễ ăn",
    "image": "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80",
    "desc": "Nui xào bò được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị"
    ],
    "story": "Nui xào bò mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 18,
    "name": "Gỏi cuốn tôm thịt",
    "category": "Healthy",
    "price": 45000,
    "tag": "Tươi mát",
    "image": "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
    "desc": "Gỏi cuốn tôm thịt được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nguyên liệu chính",
      "Gia vị cơ bản",
      "Rau ăn kèm",
      "Sốt đặc trưng"
    ],
    "story": "Gỏi cuốn tôm thịt là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 19,
    "name": "Cơm cá hồi áp chảo",
    "category": "Healthy",
    "price": 89000,
    "tag": "Premium",
    "image": "https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm cá hồi áp chảo được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm cá hồi áp chảo là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 20,
    "name": "Mì xào hải sản",
    "category": "Mì",
    "price": 66000,
    "tag": "Đậm đà",
    "image": "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80",
    "desc": "Mì xào hải sản được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Sợi mì/bún",
      "Nước dùng",
      "Rau thơm"
    ],
    "story": "Mì xào hải sản gợi cảm giác ấm bụng và quen thuộc. Món phù hợp cho những lúc muốn ăn nhanh nhưng vẫn cần hương vị rõ ràng, có nước dùng hoặc sốt làm điểm nhấn."
  },
  {
    "id": 21,
    "name": "Cơm thịt kho trứng",
    "category": "Cơm",
    "price": 56000,
    "tag": "Nhà làm",
    "image": "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm thịt kho trứng được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm thịt kho trứng là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 22,
    "name": "Khoai tây chiên phô mai",
    "category": "Ăn vặt",
    "price": 35000,
    "tag": "Ăn vặt",
    "image": "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80",
    "desc": "Khoai tây chiên phô mai được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nguyên liệu chính",
      "Gia vị cơ bản",
      "Rau ăn kèm",
      "Sốt đặc trưng"
    ],
    "story": "Khoai tây chiên phô mai là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 23,
    "name": "Sữa chua trái cây",
    "category": "Đồ uống",
    "price": 42000,
    "tag": "Fresh",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Sữa chua trái cây được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nguyên liệu chính",
      "Gia vị cơ bản",
      "Rau ăn kèm",
      "Sốt đặc trưng"
    ],
    "story": "Sữa chua trái cây là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 24,
    "name": "Nước ép cam",
    "category": "Đồ uống",
    "price": 33000,
    "tag": "Vitamin",
    "image": "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
    "desc": "Nước ép cam được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Nước ép cam được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 25,
    "name": "Cơm gà teriyaki",
    "category": "Cơm",
    "price": 69000,
    "tag": "Kiểu Nhật",
    "image": "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm gà teriyaki được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Thịt gà",
      "Gia vị ướp"
    ],
    "story": "Cơm gà teriyaki là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 26,
    "name": "Ramen gà nấm",
    "category": "Mì",
    "price": 74000,
    "tag": "Ấm bụng",
    "image": "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
    "desc": "Ramen gà nấm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Ramen gà nấm được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 27,
    "name": "Tacos gà sốt salsa",
    "category": "Fastfood",
    "price": 62000,
    "tag": "Mới",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Tacos gà sốt salsa được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành"
    ],
    "story": "Tacos gà sốt salsa là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 28,
    "name": "Bánh xèo mini",
    "category": "Ăn vặt",
    "price": 49000,
    "tag": "Giòn rụm",
    "image": "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
    "desc": "Bánh xèo mini được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Vỏ bánh",
      "Nhân chính",
      "Sốt ăn kèm"
    ],
    "story": "Bánh xèo mini là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 29,
    "name": "Đậu hũ non sốt Tứ Xuyên",
    "category": "Món chay",
    "price": 53000,
    "tag": "Cay nhẹ",
    "image": "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80",
    "desc": "Đậu hũ non sốt Tứ Xuyên được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nấm tươi",
      "Rau củ",
      "Đậu hũ / nước dùng rau củ"
    ],
    "story": "Đậu hũ non sốt Tứ Xuyên là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 30,
    "name": "Súp bí đỏ",
    "category": "Healthy",
    "price": 39000,
    "tag": "Nhẹ bụng",
    "image": "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
    "desc": "Súp bí đỏ được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nguyên liệu chính",
      "Gia vị cơ bản",
      "Rau ăn kèm",
      "Sốt đặc trưng"
    ],
    "story": "Súp bí đỏ là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 31,
    "name": "Cơm cuộn rong biển",
    "category": "Healthy",
    "price": 45000,
    "tag": "Tiện lợi",
    "image": "https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm cuộn rong biển được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm cuộn rong biển là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 32,
    "name": "Gà nướng lá chanh",
    "category": "Gà",
    "price": 76000,
    "tag": "Thơm",
    "image": "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80",
    "desc": "Gà nướng lá chanh được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành"
    ],
    "story": "Gà nướng lá chanh là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 33,
    "name": "Mì trộn xá xíu",
    "category": "Mì",
    "price": 64000,
    "tag": "Đặc biệt",
    "image": "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=900&q=80",
    "desc": "Mì trộn xá xíu được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Sợi mì/bún",
      "Nước dùng",
      "Rau thơm"
    ],
    "story": "Mì trộn xá xíu gợi cảm giác ấm bụng và quen thuộc. Món phù hợp cho những lúc muốn ăn nhanh nhưng vẫn cần hương vị rõ ràng, có nước dùng hoặc sốt làm điểm nhấn."
  },
  {
    "id": 34,
    "name": "Cơm tấm bì chả",
    "category": "Cơm",
    "price": 61000,
    "tag": "Truyền thống",
    "image": "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm tấm bì chả được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm tấm bì chả là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 35,
    "name": "Sinh tố xoài",
    "category": "Đồ uống",
    "price": 38000,
    "tag": "Mát lạnh",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Sinh tố xoài được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Sinh tố xoài được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 36,
    "name": "Matcha latte",
    "category": "Đồ uống",
    "price": 45000,
    "tag": "Ngọt nhẹ",
    "image": "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
    "desc": "Matcha latte được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Matcha latte được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 37,
    "name": "Cơm bò sốt vang",
    "category": "Cơm",
    "price": 72000,
    "tag": "Đậm sốt",
    "image": "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm bò sốt vang được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Thịt bò",
      "Hành tây"
    ],
    "story": "Cơm bò sốt vang mang phong cách no lâu, hợp với khách cần một bữa chắc bụng. Hương vị chính đến từ thịt bò và phần sốt đậm, tạo cảm giác mạnh mẽ nhưng vẫn dễ ăn."
  },
  {
    "id": 38,
    "name": "Miến gà nấm",
    "category": "Mì",
    "price": 54000,
    "tag": "Thanh",
    "image": "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
    "desc": "Miến gà nấm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Miến gà nấm được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 39,
    "name": "Pizza mini hải sản",
    "category": "Fastfood",
    "price": 85000,
    "tag": "Phô mai",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Pizza mini hải sản được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Vỏ bánh",
      "Nhân chính",
      "Sốt ăn kèm"
    ],
    "story": "Pizza mini hải sản là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 40,
    "name": "Gà popcorn",
    "category": "Ăn vặt",
    "price": 48000,
    "tag": "Giòn",
    "image": "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
    "desc": "Gà popcorn được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành"
    ],
    "story": "Gà popcorn là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 41,
    "name": "Cơm chay rau củ nướng",
    "category": "Món chay",
    "price": 50000,
    "tag": "Ngày rằm",
    "image": "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm chay rau củ nướng được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Cơm chay rau củ nướng được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 42,
    "name": "Salad bơ trứng",
    "category": "Healthy",
    "price": 62000,
    "tag": "Healthy",
    "image": "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
    "desc": "Salad bơ trứng được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Rau xanh",
      "Protein ít béo",
      "Sốt nhẹ"
    ],
    "story": "Salad bơ trứng dành cho khách muốn ăn nhẹ mà vẫn đủ chất. Câu chuyện của món nằm ở sự cân bằng giữa rau xanh, protein và phần sốt vừa đủ để không làm mất cảm giác tươi."
  },
  {
    "id": 43,
    "name": "Cơm cá basa sốt cà",
    "category": "Cơm",
    "price": 58000,
    "tag": "Dễ ăn",
    "image": "https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm cá basa sốt cà được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm cá basa sốt cà là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 44,
    "name": "Hủ tiếu Nam Vang",
    "category": "Mì",
    "price": 63000,
    "tag": "Nước lèo",
    "image": "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80",
    "desc": "Hủ tiếu Nam Vang được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Sợi mì/bún",
      "Nước dùng",
      "Rau thơm"
    ],
    "story": "Hủ tiếu Nam Vang là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 45,
    "name": "Bánh bao xá xíu",
    "category": "Ăn vặt",
    "price": 32000,
    "tag": "Nóng",
    "image": "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=900&q=80",
    "desc": "Bánh bao xá xíu được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Vỏ bánh",
      "Nhân chính",
      "Sốt ăn kèm"
    ],
    "story": "Bánh bao xá xíu là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 46,
    "name": "Trà vải hoa hồng",
    "category": "Đồ uống",
    "price": 36000,
    "tag": "Thơm",
    "image": "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80",
    "desc": "Trà vải hoa hồng được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Trà vải hoa hồng được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 47,
    "name": "Cà phê sữa đá",
    "category": "Đồ uống",
    "price": 30000,
    "tag": "Đậm",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Cà phê sữa đá được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Cà phê sữa đá được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 48,
    "name": "Soda chanh bạc hà",
    "category": "Đồ uống",
    "price": 34000,
    "tag": "Fresh",
    "image": "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
    "desc": "Soda chanh bạc hà được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Soda chanh bạc hà được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  },
  {
    "id": 49,
    "name": "Cơm gà xối mỡ",
    "category": "Cơm",
    "price": 67000,
    "tag": "Giòn",
    "image": "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm gà xối mỡ được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Thịt gà",
      "Gia vị ướp"
    ],
    "story": "Cơm gà xối mỡ là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 50,
    "name": "Mì vịt tiềm",
    "category": "Mì",
    "price": 82000,
    "tag": "Bổ dưỡng",
    "image": "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?auto=format&fit=crop&w=900&q=80",
    "desc": "Mì vịt tiềm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Sợi mì/bún",
      "Nước dùng",
      "Rau thơm"
    ],
    "story": "Mì vịt tiềm gợi cảm giác ấm bụng và quen thuộc. Món phù hợp cho những lúc muốn ăn nhanh nhưng vẫn cần hương vị rõ ràng, có nước dùng hoặc sốt làm điểm nhấn."
  },
  {
    "id": 51,
    "name": "Sandwich cá ngừ",
    "category": "Fastfood",
    "price": 52000,
    "tag": "Nhanh",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Sandwich cá ngừ được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Vỏ bánh",
      "Nhân chính",
      "Sốt ăn kèm"
    ],
    "story": "Sandwich cá ngừ là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 52,
    "name": "Nem chua rán",
    "category": "Ăn vặt",
    "price": 42000,
    "tag": "Ăn vặt",
    "image": "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80",
    "desc": "Nem chua rán được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nguyên liệu chính",
      "Gia vị cơ bản",
      "Rau ăn kèm",
      "Sốt đặc trưng"
    ],
    "story": "Nem chua rán là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 53,
    "name": "Lẩu chay mini",
    "category": "Món chay",
    "price": 79000,
    "tag": "Ấm bụng",
    "image": "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80",
    "desc": "Lẩu chay mini được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nấm tươi",
      "Rau củ",
      "Đậu hũ / nước dùng rau củ"
    ],
    "story": "Lẩu chay mini được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 54,
    "name": "Ức gà sốt chanh dây",
    "category": "Healthy",
    "price": 69000,
    "tag": "Ít béo",
    "image": "https://images.unsplash.com/photo-1603133872878-684f208fb84b?auto=format&fit=crop&w=900&q=80",
    "desc": "Ức gà sốt chanh dây được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt gà",
      "Gia vị ướp",
      "Tiêu / tỏi / hành"
    ],
    "story": "Ức gà sốt chanh dây là món được tạo cho những bữa ăn nhanh nhưng vẫn đủ năng lượng. Điểm nhấn nằm ở phần gà được nêm đậm vừa phải, ăn cùng cơm hoặc rau để giữ vị cân bằng."
  },
  {
    "id": 55,
    "name": "Cơm rang kim chi",
    "category": "Cơm",
    "price": 59000,
    "tag": "Cay nhẹ",
    "image": "https://images.unsplash.com/photo-1562967914-608f82629710?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm rang kim chi được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng"
    ],
    "story": "Cơm rang kim chi là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 56,
    "name": "Udon bò nấm",
    "category": "Mì",
    "price": 76000,
    "tag": "Kiểu Nhật",
    "image": "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80",
    "desc": "Udon bò nấm được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Thịt bò",
      "Hành tây",
      "Sốt đậm vị",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Udon bò nấm được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 57,
    "name": "Bánh gạo cay",
    "category": "Ăn vặt",
    "price": 45000,
    "tag": "Hot",
    "image": "https://images.unsplash.com/photo-1552611052-33e04de081de?auto=format&fit=crop&w=900&q=80",
    "desc": "Bánh gạo cay được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Vỏ bánh",
      "Nhân chính",
      "Sốt ăn kèm"
    ],
    "story": "Bánh gạo cay là một món trong menu được thiết kế để dễ ăn, dễ nhớ và phù hợp nhiều nhóm khách. Món có câu chuyện ngắn xoay quanh sự tiện lợi, hương vị gần gũi và cảm giác thoải mái khi dùng bữa."
  },
  {
    "id": 58,
    "name": "Cơm chay kho quẹt",
    "category": "Món chay",
    "price": 53000,
    "tag": "Việt Nam",
    "image": "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80",
    "desc": "Cơm chay kho quẹt được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Cơm nóng",
      "Rau ăn kèm",
      "Nước sốt đặc trưng",
      "Nấm tươi",
      "Rau củ"
    ],
    "story": "Cơm chay kho quẹt được xây dựng như một lựa chọn nhẹ bụng cho ngày rằm hoặc những hôm muốn ăn thanh đạm. Món tập trung vào vị ngọt tự nhiên từ rau củ và nấm, tạo cảm giác sạch, ấm và dễ chịu."
  },
  {
    "id": 59,
    "name": "Poke bowl cá hồi",
    "category": "Healthy",
    "price": 99000,
    "tag": "Premium",
    "image": "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80",
    "desc": "Poke bowl cá hồi được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Rau xanh",
      "Protein ít béo",
      "Sốt nhẹ"
    ],
    "story": "Poke bowl cá hồi dành cho khách muốn ăn nhẹ mà vẫn đủ chất. Câu chuyện của món nằm ở sự cân bằng giữa rau xanh, protein và phần sốt vừa đủ để không làm mất cảm giác tươi."
  },
  {
    "id": 60,
    "name": "Trà sữa olong",
    "category": "Đồ uống",
    "price": 39000,
    "tag": "Ngọt vừa",
    "image": "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=900&q=80",
    "desc": "Trà sữa olong được chuẩn bị theo phong cách quán, phù hợp dùng tại chỗ hoặc giao tận nơi.",
    "ingredients": [
      "Nền đồ uống",
      "Đá viên",
      "Topping / hương vị tự nhiên"
    ],
    "story": "Trà sữa olong được chọn làm món đi kèm để làm dịu vị sau bữa ăn. Món uống này tạo cảm giác thư giãn, tươi mới và giúp trải nghiệm tại quán vui hơn."
  }
];


function getCurrentMenuPage() {
  const match = window.location.pathname.match(/menu-page-(\d+)\.html/);
  return match ? Number(match[1]) : 1;
}

function getCurrentPageItems() {
  const page = getCurrentMenuPage();
  const start = (page - 1) * 12;
  return FULL_MENU_ITEMS.slice(start, start + 12);
}

function escapeHtml(value) {
  return String(value)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

function menuFoodCardTemplate(item) {
  const ingredients = Array.isArray(item.ingredients) ? item.ingredients.join(', ') : item.ingredients;
  const safeName = String(item.name).replaceAll("'", "\\'");
  return `
    <article class="food-card" onclick="openFoodDetail(this)"
      data-category="${escapeHtml(item.category.toLowerCase())}"
      data-name="${escapeHtml(item.name.toLowerCase())}"
      data-ingredients="${escapeHtml(ingredients)}"
      data-story="${escapeHtml(item.story)}">
      <div class="food-img">
        <img src="${escapeHtml(item.image)}" alt="${escapeHtml(item.name)}" />
        <span class="food-tag">${escapeHtml(item.tag)}</span>
      </div>
      <div class="food-body">
        <h3>${escapeHtml(item.name)}</h3>
        <p>${escapeHtml(item.desc)}</p>
        <div class="food-bottom">
          <span class="price">${formatVND(item.price)}</span>
          <button class="small-btn" onclick="event.stopPropagation(); addToCart('${safeName}', ${item.price}, '${escapeHtml(item.image)}', ${item.id || 0})">Thêm</button>
        </div>
      </div>
    </article>
  `;
}

function renderMenuItems(items, modeText) {
  const grid = document.getElementById('menuFoodGrid');
  if (!grid) return;

  grid.innerHTML = items.map(menuFoodCardTemplate).join('');

  let resultInfo = document.getElementById('menuResultInfo');
  if (!resultInfo) {
    resultInfo = document.createElement('p');
    resultInfo.id = 'menuResultInfo';
    resultInfo.className = 'menu-result-info';
    grid.parentElement.insertBefore(resultInfo, grid);
  }

  resultInfo.textContent = modeText;
}

function showMenuCategory(category, button) {
  document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.classList.remove('active');
  });
  if (button) button.classList.add('active');

  const pagination = document.getElementById('menuPagination');
  const searchInput = document.getElementById('searchInput');
  if (searchInput) searchInput.value = '';

  if (category === 'all') {
    const items = getCurrentPageItems();
    renderMenuItems(items, `Đang hiển thị 12 món của trang ${getCurrentMenuPage()}.`);
    if (pagination) pagination.style.display = 'flex';
    currentCategory = 'all';
    return;
  }

  const normalized = category.toLowerCase();
  const items = FULL_MENU_ITEMS.filter(item => item.category.toLowerCase() === normalized);

  renderMenuItems(items, `Đang hiển thị tất cả ${items.length} món thuộc danh mục "${button ? button.textContent.trim() : category}" trong toàn bộ 5 trang menu.`);
  if (pagination) pagination.style.display = 'none';
  currentCategory = category;
}

// Override search on menu page:
// - Nếu đang ở "Tất cả": tìm trong 12 món của trang hiện tại.
// - Nếu đang chọn danh mục: tìm trong toàn bộ danh mục đã chọn của 60 món.
const oldFilterFoods = filterFoods;
filterFoods = function() {
  const grid = document.getElementById('menuFoodGrid');
  const searchInput = document.getElementById('searchInput');

  if (!grid || !searchInput || typeof FULL_MENU_ITEMS === 'undefined') {
    oldFilterFoods();
    return;
  }

  const searchValue = searchInput.value.toLowerCase().trim();
  let items;

  if (currentCategory === 'all') {
    items = getCurrentPageItems();
  } else {
    items = FULL_MENU_ITEMS.filter(item => item.category.toLowerCase() === currentCategory.toLowerCase());
  }

  if (searchValue) {
    items = items.filter(item => item.name.toLowerCase().includes(searchValue));
  }

  const mode = currentCategory === 'all'
    ? `Tìm thấy ${items.length} món trong trang ${getCurrentMenuPage()}.`
    : `Tìm thấy ${items.length} món trong danh mục đang chọn trên toàn bộ menu.`;

  renderMenuItems(items, mode);
};


// Defensive binding for menu filter buttons.
// This keeps the filter working even if inline onclick is removed later.
document.addEventListener('DOMContentLoaded', () => {
  const menuGrid = document.getElementById('menuFoodGrid');
  if (!menuGrid) return;

  document.querySelectorAll('.filters .filter-btn').forEach((button) => {
    const text = button.textContent.trim().toLowerCase();
    const category = text === 'tất cả' ? 'all' : text;

    button.addEventListener('click', function() {
      if (typeof showMenuCategory === 'function') {
        showMenuCategory(category, button);
      }
    });
  });
});


// ASP.NET Core MVC compatibility layer
// Menu page is now /Home/Menu?page=1 instead of menu-page-1.html.
function getMvcCurrentMenuPage() {
  const grid = document.getElementById('menuFoodGrid');
  const fromData = grid ? Number(grid.dataset.currentPage) : 0;
  const fromQuery = Number(new URLSearchParams(window.location.search).get('page'));
  const page = fromData || fromQuery || 1;
  return Math.min(Math.max(page, 1), 5);
}

// Override old static-HTML page detector.
getCurrentMenuPage = getMvcCurrentMenuPage;

function renderMvcHomeBestSellers() {
  const grid = document.getElementById('homeBestSellerGrid');
  if (!grid || typeof FULL_MENU_ITEMS === 'undefined' || typeof menuFoodCardTemplate !== 'function') return;
  const bestIds = [1, 2, 3, 7, 13, 25];
  const items = FULL_MENU_ITEMS.filter(item => bestIds.includes(item.id));
  grid.innerHTML = items.map(menuFoodCardTemplate).join('');
}

function initMvcMenuPage() {
  const grid = document.getElementById('menuFoodGrid');
  if (!grid || typeof getCurrentPageItems !== 'function') return;

  currentCategory = 'all';
  renderMenuItems(getCurrentPageItems(), `Đang hiển thị 12 món của trang ${getCurrentMenuPage()}.`);

  document.querySelectorAll('.filters .filter-btn').forEach((button) => {
    const category = button.dataset.category || (button.textContent.trim().toLowerCase() === 'tất cả' ? 'all' : button.textContent.trim().toLowerCase());
    button.onclick = function() {
      showMenuCategory(category, button);
    };
  });
}

document.addEventListener('DOMContentLoaded', () => {
  renderMvcHomeBestSellers();
  initMvcMenuPage();
});

