#!/usr/bin/env bash

set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
    echo "用法: ./scripts/new-plugin.sh <PluginName> [DisplayName]" >&2
    echo "示例: ./scripts/new-plugin.sh HelloPlugin 你好插件" >&2
    exit 1
fi

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
template_dir="$repo_root/templates/ShiroBot.PluginTemplate"

plugin_name="$1"
display_name="${2:-$plugin_name}"

if [[ ! "$plugin_name" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]]; then
    echo "PluginName 只能包含字母、数字、下划线，且不能以数字开头。" >&2
    exit 1
fi

target_dir="$repo_root/$plugin_name"

if [[ ! -d "$template_dir" ]]; then
    echo "模板目录不存在: $template_dir" >&2
    exit 1
fi

if [[ -e "$target_dir" ]]; then
    echo "目标目录已存在: $target_dir" >&2
    exit 1
fi

cp -R "$template_dir" "$target_dir"

find "$target_dir" -type f | while IFS= read -r file; do
    perl -0pi -e \
        "s/__PLUGIN_NAME__/${plugin_name}/g; s/__PLUGIN_DISPLAY_NAME__/${display_name}/g" \
        "$file"
done

if [[ -f "$target_dir/ShiroBot.PluginTemplate.csproj" ]]; then
    mv "$target_dir/ShiroBot.PluginTemplate.csproj" "$target_dir/$plugin_name.csproj"
fi

if [[ -f "$target_dir/PluginTemplate.cs" ]]; then
    mv "$target_dir/PluginTemplate.cs" "$target_dir/${plugin_name}.cs"
fi

if [[ -f "$target_dir/PluginTemplateConfig.cs" ]]; then
    mv "$target_dir/PluginTemplateConfig.cs" "$target_dir/${plugin_name}Config.cs"
fi

echo "已创建插件模板: $target_dir"
echo "下一步:"
echo "  1. 把 $plugin_name/$plugin_name.csproj 加到解决方案"
echo "  2. 运行: dotnet build ShiroBot.slnx"
echo "  3. 按需修改 Metadata、命令路由和配置项"
