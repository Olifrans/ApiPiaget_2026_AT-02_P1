// ==========================================
// CONFIGURAÇÃO E ESTADO
// ==========================================
const API_BASE = 'https://localhost:7274/api';


//Docker expõe a API na porta 8080 via HTTP (não HTTPS, para simplificar o ambiente local de container):
//const API_BASE = 'http://localhost:8080/api';


const state = {
    currentView: 'dashboard',
    escolas: [],
    alunos: [],
    professores: [],
    filteredData: [],
    currentPage: 1,
    pageSize: 10
};

// Mapeamento explícito para evitar bugs de plural/singular (ex: professores -> professore)
const ENTITY_MAP = {
    'escolas': 'escola',
    'alunos': 'aluno',
    'professores': 'professor'
};

// ==========================================
// UTILITÁRIOS
// ==========================================
const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => document.querySelectorAll(selector);

function escapeHtml(text) {
    if (!text) return 'N/A';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function toggleLoading(show) {
    const loader = $('#loadingIndicator');
    const table = $('#dataTable');
    if (loader) loader.style.display = show ? 'block' : 'none';
    if (table) table.style.opacity = show ? '0.5' : '1';
}

function showToast(message, type = 'success') {
    const existing = document.querySelector('.toast');
    if (existing) existing.remove();
    
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<i class="fas ${type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle'}"></i> ${escapeHtml(message)}`;
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(20px)';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ==========================================
// CAMADA DE API
// ==========================================
async function apiRequest(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: { 
            'Accept': 'application/json', 
            'Content-Type': 'application/json' 
        }
    };
    
    if (body) {
        options.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(`${API_BASE}/${endpoint}`, options);
        
        if (response.status === 204 || method === 'DELETE') {
            return true;
        }
        
        const text = await response.text();
        if (!response.ok) {
            throw new Error(`Erro ${response.status}: ${text || 'Falha na requisição'}`);
        }
        
        return text ? JSON.parse(text) : null;
    } catch (error) {
        console.error('API Error:', error);
        showToast(error.message, 'error');
        throw error;
    }
}

// ==========================================
// CARREGAMENTO DE DADOS
// ==========================================
async function loadData() {
    toggleLoading(true);
    try {
        // Promise.all com catch individual para não quebrar tudo se uma tabela estiver vazia
        const [escolas, alunos, professores] = await Promise.all([
            apiRequest('Escola').catch(() => []),
            apiRequest('Aluno').catch(() => []),
            apiRequest('Professor').catch(() => [])
        ]);

        state.escolas = escolas || [];
        state.alunos = (alunos || []).map(a => ({ ...a, tipo: 'aluno' }));
        state.professores = (professores || []).map(p => ({ ...p, tipo: 'professor' }));

        updateStats();
        populateEscolaSelect();
        applyFilters();
    } catch (error) {
        console.error('Erro crítico ao carregar dados:', error);
    } finally {
        toggleLoading(false);
    }
}

function updateStats() {
    const elEscolas = $('#statEscolas');
    const elAlunos = $('#statAlunos');
    const elProfessores = $('#statProfessores');
    
    if (elEscolas) elEscolas.textContent = state.escolas.length;
    if (elAlunos) elAlunos.textContent = state.alunos.length;
    if (elProfessores) elProfessores.textContent = state.professores.length;
}

function populateEscolaSelect() {
    const select = $('#formEscolaId');
    if (!select) return;
    
    select.innerHTML = '<option value="">Selecione uma escola...</option>';
    state.escolas.forEach(escola => {
        const option = document.createElement('option');
        option.value = escola.id;
        option.textContent = escola.escolaNome || 'Escola sem nome';
        select.appendChild(option);
    });
}

// ==========================================
// RENDERIZAÇÃO E FILTROS
// ==========================================
function applyFilters() {
    let data = [];
    
    if (state.currentView === 'alunos') data = [...state.alunos];
    else if (state.currentView === 'professores') data = [...state.professores];
    else if (state.currentView === 'escolas') data = [...state.escolas];
    else {
        // Dashboard: esconde tabela e paginação
        const table = $('#dataTable');
        const pagination = $('#paginationControls');
        const emptyState = $('#emptyState');
        if (table) table.style.display = 'none';
        if (pagination) pagination.style.display = 'none';
        if (emptyState) emptyState.style.display = 'none';
        return;
    }

    // Mostra elementos de tabela
    const table = $('#dataTable');
    const pagination = $('#paginationControls');
    if (table) table.style.display = 'table';
    if (pagination) pagination.style.display = 'flex';

    // Filtro de busca
    const searchTerm = $('#searchInput')?.value.toLowerCase().trim() || '';
    if (searchTerm) {
        data = data.filter(item => {
            const nome = item.escolaNome || item.alunoNome || item.professorNome || '';
            const email = item.escolaEmail || item.alunoEmail || item.professorEmail || '';
            const matricula = item.alunoMatricula || '';
            const disciplina = item.professorDisciplina || '';
            
            return nome.toLowerCase().includes(searchTerm) || 
                   email.toLowerCase().includes(searchTerm) || 
                   matricula.toLowerCase().includes(searchTerm) ||
                   disciplina.toLowerCase().includes(searchTerm);
        });
    }

    state.filteredData = data;
    state.currentPage = 1;
    renderTable();
}

function renderTable() {
    const thead = $('#tableHead');
    const tbody = $('#tableBody');
    const emptyState = $('#emptyState');

    if (!state.filteredData || state.filteredData.length === 0) {
        if (tbody) tbody.innerHTML = '';
        if (emptyState) emptyState.style.display = 'block';
        updatePagination(0);
        return;
    }
    
    if (emptyState) emptyState.style.display = 'none';

    // 1. Configurar Cabeçalho Dinâmico
    if (state.currentView === 'escolas') {
        thead.innerHTML = `<tr>
            <th>Nome da Escola</th>
            <th>E-mail</th>
            <th>Telefone</th>
            <th>Qtd. Alunos</th>
            <th>Qtd. Professores</th>
            <th>Ações</th>
        </tr>`;
    } else {
        const extraCol = state.currentView === 'alunos' ? '<th>Matrícula</th>' : '<th>Disciplina</th>';
        thead.innerHTML = `<tr>
            <th>Nome</th>
            <th>E-mail</th>
            <th>Telefone</th>
            <th>Escola Vinculada</th>
            ${extraCol}
            <th>Ações</th>
        </tr>`;
    }

    // 2. Paginação
    const start = (state.currentPage - 1) * state.pageSize;
    const end = start + state.pageSize;
    const pageData = state.filteredData.slice(start, end);

    // 3. Renderizar Linhas
    tbody.innerHTML = pageData.map(item => {
        if (state.currentView === 'escolas') {
            return `<tr>
                <td><strong>${escapeHtml(item.escolaNome)}</strong></td>
                <td>${escapeHtml(item.escolaEmail)}</td>
                <td>${escapeHtml(item.escolaTelefone)}</td>
                <td><span class="badge badge-aluno">${item.totalAlunos || 0}</span></td>
                <td><span class="badge badge-professor">${item.totalProfessores || 0}</span></td>
                <td>${getActions(item.id, 'escola')}</td>
            </tr>`;
        } else {
            const isAluno = item.tipo === 'aluno';
            const prefix = isAluno ? 'aluno' : 'professor';
            
            return `<tr>
                <td><strong>${escapeHtml(item[`${prefix}Nome`])}</strong></td>
                <td>${escapeHtml(item[`${prefix}Email`])}</td>
                <td>${escapeHtml(item[`${prefix}Telefone`])}</td>
                <td>${escapeHtml(item.escolaNome || 'Não vinculada')}</td>
                <td><span class="badge ${isAluno ? 'badge-aluno' : 'badge-professor'}">
                    ${escapeHtml(isAluno ? item.alunoMatricula : item.professorDisciplina)}
                </span></td>
                <td>${getActions(item.id, item.tipo)}</td>
            </tr>`;
        }
    }).join('');

    updatePagination(state.filteredData.length);
}

function getActions(id, type) {
    return `
        <button class="action-btn btn-edit" onclick="openEditModal('${id}', '${type}')" title="Editar">
            <i class="fas fa-edit"></i>
        </button>
        <button class="action-btn btn-delete" onclick="deleteItem('${id}', '${type}')" title="Excluir">
            <i class="fas fa-trash"></i>
        </button>
    `;
}

function updatePagination(totalItems) {
    const totalPages = Math.max(1, Math.ceil(totalItems / state.pageSize));
    const pageInfo = $('#pageInfo');
    const prevBtn = $('#prevPage');
    const nextBtn = $('#nextPage');

    if (pageInfo) pageInfo.textContent = `Página ${state.currentPage} de ${totalPages}`;
    if (prevBtn) prevBtn.disabled = state.currentPage <= 1;
    if (nextBtn) nextBtn.disabled = state.currentPage >= totalPages;
}

// ==========================================
// MODAL E FORMULÁRIO (LÓGICA CORRIGIDA)
// ==========================================
function openModal(type, item = null) {
    const formType = $('#formType');
    const formId = $('#formId');
    const modalTitle = $('#modalTitle');
    
    if (!formType || !formId) return;

    formType.value = type;
    formId.value = item ? item.id : '';
    modalTitle.textContent = item ? `Editar ${capitalize(type)}` : `Novo ${capitalize(type)}`;
    
    // Mostrar/ocultar campos baseado no atributo data-entities do HTML
    $$('.form-group').forEach(group => {
        const entities = group.getAttribute('data-entities').split(',');
        const input = group.querySelector('input, select');
        
        if (entities.includes(type)) {
            group.classList.remove('hidden');
            if (input) input.required = true;
        } else {
            group.classList.add('hidden');
            if (input) {
                input.required = false;
                input.value = ''; // Limpa valor ao ocultar
            }
        }
    });

    // Preencher dados se for edição
    if (item) {
        if (type === 'escola') {
            $('#formNome').value = item.escolaNome || '';
            $('#formEmail').value = item.escolaEmail || '';
            $('#formTelefone').value = item.escolaTelefone || '';
            $('#formEndereco').value = item.escolaEndereco || '';
        } else {
            const prefix = type === 'aluno' ? 'aluno' : 'professor';
            $('#formNome').value = item[`${prefix}Nome`] || '';
            $('#formEmail').value = item[`${prefix}Email`] || '';
            $('#formTelefone').value = item[`${prefix}Telefone`] || '';
            $('#formEscolaId').value = item.escolaId || '';
            
            if (type === 'aluno') $('#formMatricula').value = item.alunoMatricula || '';
            if (type === 'professor') $('#formDisciplina').value = item.professorDisciplina || '';
        }
    } else {
        $('#formModal').reset();
    }

    $('#modal').classList.add('active');
}

// Função global para ser chamada pelo onclick do HTML
window.openEditModal = (id, type) => {
    let item;
    if (type === 'escola') item = state.escolas.find(e => e.id === id);
    else if (type === 'aluno') item = state.alunos.find(a => a.id === id);
    else if (type === 'professor') item = state.professores.find(p => p.id === id);
    
    if (item) {
        openModal(type, item);
    } else {
        showToast('Registro não encontrado.', 'error');
    }
};

function closeModal() {
    $('#modal').classList.remove('active');
    $('#formModal').reset();
}

async function handleFormSubmit(e) {
    e.preventDefault();
    
    const type = $('#formType').value;
    const id = $('#formId').value;
    const isEdit = !!id;
    const payload = {};

    // Montar payload exatamente como os DTOs da API C# esperam
    if (type === 'escola') {
        payload.escolaNome = $('#formNome').value.trim();
        payload.escolaEmail = $('#formEmail').value.trim();
        payload.escolaTelefone = $('#formTelefone').value.trim();
        payload.escolaEndereco = $('#formEndereco').value.trim();
    } else if (type === 'aluno') {
        payload.alunoNome = $('#formNome').value.trim();
        payload.alunoEmail = $('#formEmail').value.trim();
        payload.alunoTelefone = $('#formTelefone').value.trim();
        payload.alunoMatricula = $('#formMatricula').value.trim();
        payload.escolaId = $('#formEscolaId').value;
    } else if (type === 'professor') {
        payload.professorNome = $('#formNome').value.trim();
        payload.professorEmail = $('#formEmail').value.trim();
        payload.professorTelefone = $('#formTelefone').value.trim();
        payload.professorDisciplina = $('#formDisciplina').value.trim();
        payload.escolaId = $('#formEscolaId').value;
    }

    // Adiciona o ID no payload para o PUT da API (conforme seu controller C#)
    if (isEdit) {
        payload.id = id;
    }

    try {
        const endpoint = capitalize(type); // 'Escola', 'Aluno', 'Professor'
        
        if (isEdit) {
            await apiRequest(`${endpoint}/${id}`, 'PUT', payload);
            showToast(`${capitalize(type)} atualizado com sucesso!`, 'success');
        } else {
            await apiRequest(endpoint, 'POST', payload);
            showToast(`${capitalize(type)} cadastrado com sucesso!`, 'success');
        }
        
        closeModal();
        await loadData();
    } catch (error) {
        console.error('Erro ao salvar:', error);
    }
}

window.deleteItem = async (id, type) => {
    const entityName = type === 'escola' ? 'esta escola' : (type === 'aluno' ? 'este aluno' : 'este professor');
    
    if (!confirm(`Tem certeza que deseja excluir ${entityName}? Esta ação não pode ser desfeita.`)) {
        return;
    }
    
    try {
        const endpoint = capitalize(type);
        await apiRequest(`${endpoint}/${id}`, 'DELETE');
        showToast('Registro excluído com sucesso!', 'success');
        await loadData();
    } catch (error) {
        // Erro já exibido pelo showToast dentro do apiRequest
    }
};

// ==========================================
// EVENT LISTENERS
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    // 1. Navegação (Abas)
    $$('.nav-item').forEach(btn => {
        btn.addEventListener('click', () => {
            $$('.nav-item').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            
            state.currentView = btn.dataset.view;
            
            const titles = {
                dashboard: ['📊 Dashboard', 'Visão geral do sistema'],
                escolas: ['🏫 Escolas', 'Gerencie as unidades de ensino'],
                alunos: ['👨‍🎓 Alunos', 'Gerencie o corpo discente'],
                professores: ['👨‍🏫 Professores', 'Gerencie o corpo docente']
            };
            
            $('#pageTitle').textContent = titles[state.currentView][0];
            $('#pageSubtitle').textContent = titles[state.currentView][1];
            
            const searchBar = $('#searchBar');
            const btnAdd = $('#btnAdd');
            
            if (state.currentView === 'dashboard') {
                if (searchBar) searchBar.style.display = 'none';
                if (btnAdd) btnAdd.style.display = 'none';
            } else {
                if (searchBar) searchBar.style.display = 'block';
                if (btnAdd) {
                    btnAdd.style.display = 'inline-flex';
                    // CORREÇÃO DEFINITIVA DO BUG DO PLURAL
                    const singularType = ENTITY_MAP[state.currentView] || state.currentView.slice(0, -1);
                    btnAdd.innerHTML = `<i class="fas fa-plus"></i> Novo ${capitalize(singularType)}`;
                }
            }
            
            applyFilters();
        });
    });

    // 2. Botão Adicionar (Usa o mapeamento seguro)
    $('#btnAdd').addEventListener('click', () => {
        if (state.currentView !== 'dashboard') {
            const singularType = ENTITY_MAP[state.currentView] || state.currentView.slice(0, -1);
            openModal(singularType);
        }
    });

    // 3. Atualizar Dados
    $('#btnRefresh').addEventListener('click', loadData);

    // 4. Busca
    $('#searchInput').addEventListener('input', () => { 
        state.currentPage = 1; 
        applyFilters(); 
    });

    // 5. Formulário
    $('#formModal').addEventListener('submit', handleFormSubmit);

    // 6. Fechar Modal
    $('#closeModal').addEventListener('click', closeModal);
    $('#cancelModal').addEventListener('click', closeModal);
    $('#modal').addEventListener('click', (e) => { 
        if (e.target === $('#modal')) closeModal(); 
    });
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') closeModal();
    });

    // 7. Paginação
    $('#prevPage').addEventListener('click', () => {
        if (state.currentPage > 1) { 
            state.currentPage--; 
            renderTable(); 
        }
    });
    
    $('#nextPage').addEventListener('click', () => {
        const totalPages = Math.ceil(state.filteredData.length / state.pageSize);
        if (state.currentPage < totalPages) { 
            state.currentPage++; 
            renderTable(); 
        }
    });

    // 8. Inicialização
    // Simula clique no dashboard para configurar o estado inicial corretamente
    const dashboardBtn = document.querySelector('.nav-item[data-view="dashboard"]');
    if (dashboardBtn) dashboardBtn.click();
    
    loadData();
});