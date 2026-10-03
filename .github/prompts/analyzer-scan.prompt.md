---
name: analyzer-scan
description: Varre uma área do código e popula o mapeamento em .analyzer/
argument-hint: Área (guild, map, town, core)
tools: ['read', 'edit']
---

Você faz a indexação de uma área do fire-bot. Não faz perguntas.

## Passos
1. Verifique se `.analyzer/areas/<área>/` existe. Se não, crie.
2. Liste `src/Behaviors/<Área>/` e `src/GameModel/Features/<Área>/`.
3. Para cada `*Task.cs`, leia e extraia:
   - nome da task
   - arquivo de modelo (procure em `GameModel/Features/`)
   - dependências de `Core/` e `UI/Config/`
   - se depende de outra task (ordem)
   - se existe duplicada em outra área
4. Escreva/atualize:
   - `.analyzer/areas/<área>/README.md` com tabela de tasks preenchida
   - `.analyzer/areas/<área>/<Task>.md` por task, com "Arquivos",
     "Depende de", "É dependência de"
   - Seções "Decisões", "Contexto" e "Abertos" ficam vazias — não invente
5. Preserve decisões já existentes nos arquivos. Não sobrescreva.
6. Devolva um resumo: tasks indexadas, arquivos criados/atualizados.

## Proibições
- Não faça perguntas.
- Não escreva em `src/`.
- Não sobrescreva decisões existentes.
- Não crie pasta por task.
- Não adicionar trailers `Co-authored-by` ou atribuições a Copilot, agentes,
  modelos ou ferramentas; o único contributor é `danilogmoura`.

Responda em português.
